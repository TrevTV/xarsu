using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using AsmResolver.PE.DotNet.Metadata.Tables;
using Cpp2IL.Core;
using Cpp2IL.Core.Model.Contexts;
using Cpp2IL.Core.OutputFormats;
using Cpp2IL.Core.Utils;
using Cpp2IL.Core.Utils.AsmResolver;
using xarsu.Generator.Extensions;

namespace xarsu.Generator;

internal class XarsuReferenceOutputFormat : AsmResolverDllOutputFormatThrowNull
{
    public override string OutputFormatId => "xarsureference";

    public override string OutputFormatName => "Xarsu Reference DLLs";

    protected override void FillMethodBody(MethodDefinition methodDefinition, MethodAnalysisContext methodContext)
    {
        if (methodContext.TryGetExtraData(out TranslatedMethodBody? translatedBody))
        {
            translatedBody.FillMethodBody(methodDefinition);
            methodContext.RemoveExtraData<TranslatedMethodBody>(); // Free up memory
        }
        else
        {
            FillMethodBodyCore(methodDefinition, methodContext);
            if (methodDefinition.CilMethodBody == null || methodDefinition.CilMethodBody.Instructions.Count == 0)
            {
                base.FillMethodBody(methodDefinition, methodContext);
            }
        }
    }

    private static void FillMethodBodyCore(MethodDefinition methodDef, MethodAnalysisContext methodCtx)
    {
        if (methodDef.IsAbstract || methodDef.IsPInvokeImpl)
            return; // no body needed for abstract or P/Invoke methods

        if (methodCtx.IsInjected)
            return; // probably has a custom body already, skip

        var appContext = Cpp2IlApi.CurrentAppContext!;

        var xarsuIl2CppStaticClass = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.IL2CPP));
        var il2cppNewObject = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.il2cpp_object_new));
        var il2cppGetIl2CppClass = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.GetIl2CppClass));
        var il2cppGetIl2CppMethod = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.GetIl2CppMethod));
        var il2cppMakeGenericMethod = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.MakeGenericMethod));
        var il2cppInvokeMethod = xarsuIl2CppStaticClass.GetMethodByName(nameof(xarsu.Reference.IL2CPP.InvokeMethod));

        var xarsuIl2CppObjectClass = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.Il2CppObject));
        var il2cppObjectGetPointer = xarsuIl2CppObjectClass.GetPropertyByName(nameof(xarsu.Reference.Il2CppObject.Pointer)).Getter!;
        var il2cppObjectWrap = xarsuIl2CppObjectClass.GetMethodByName(nameof(xarsu.Reference.Il2CppObject.Wrap))!.MakeConcreteGenericMethod([], [methodCtx.ReturnType]);

        var xarsuObjectPointerClass = appContext.ResolveTypeOrThrow(typeof(xarsu.Reference.ObjectPointer));
        var objPointerExplicitFromIntPtr = xarsuObjectPointerClass.GetExplicitConversionFrom(appContext.SystemTypes.SystemIntPtrType);
        var objPointerExplicitIntPtr = xarsuObjectPointerClass.GetExplicitConversionTo(appContext.SystemTypes.SystemIntPtrType);

        var systemTypeType = appContext.SystemTypes.SystemTypeType;
        var systemTypeGetFromHandle = systemTypeType.GetMethodByName("GetTypeFromHandle");

        var module = methodDef.DeclaringModule!;
        methodDef.CilMethodBody = new CilMethodBody();
        var il = methodDef.CilMethodBody.Instructions;

        bool isStatic = methodDef.IsStatic;
        bool isVoid = methodDef.Signature?.ReturnType is CorLibTypeSignature { ElementType: ElementType.Void };
        bool isCtor = methodDef.IsConstructor && !isStatic;

        // ----- 1. Resolve the method pointer -----
        // IL2CPP.GetIl2CppClass(assembly, ns, className) -> IntPtr klass
        // IL2CPP.GetIl2CppMethod(klass, isGeneric, name, returnType, params string[] argTypes) -> IntPtr method

        var declaringType = methodDef.DeclaringType!;

        // compute the data we need for the GetIl2CppMethod call
        string assemblyName = MiscUtils.CleanPathElement(methodCtx.DeclaringType!.DeclaringAssembly.DefaultName) + ".dll";
        string namespaceName = methodCtx.DeclaringType.DefaultNamespace ?? "";
        string className = methodCtx.DeclaringType.DefaultName ?? "";
        string methodName = methodCtx.DefaultName ?? "";
        string returnTypeName = GetTypeName(methodCtx.DefaultReturnType);
        string[] paramTypeNames = [.. methodCtx.Parameters.Select(p => GetTypeName(p.DefaultParameterType))];

        il.Add(new CilInstruction(CilOpCodes.Ldstr, assemblyName));
        il.Add(new CilInstruction(CilOpCodes.Ldstr, namespaceName));
        il.Add(new CilInstruction(CilOpCodes.Ldstr, className));
        il.Add(new CilInstruction(CilOpCodes.Call, il2cppGetIl2CppClass.ToMethodDescriptor(module)));
        // class is now found on the stack

        if (isCtor)
        {
            // setup a local variable for our object
            CilLocalVariable ctorLocalObj = new(module.CorLibTypeFactory.IntPtr);
            methodDef.CilMethodBody.LocalVariables.Add(ctorLocalObj);

            // call il2cpp_object_new to create the object, store in localObj
            il.Add(new CilInstruction(CilOpCodes.Dup)); // duplicate the class pointer for the call
            il.Add(new CilInstruction(CilOpCodes.Call, il2cppNewObject.ToMethodDescriptor(module)));
            il.Add(new CilInstruction(CilOpCodes.Stloc, ctorLocalObj)); // store the new object pointer in localObj

            // call our base ctor to initialize the Il2CppObject part of our object with the pointer
            var baseCtor = methodCtx.DeclaringType!.FindPtrCtor();
            if (baseCtor == null)
            {
                EmitNotSupported(il, module,
                    $"Constructor {methodDef.DeclaringType?.FullName}.{methodDef.Name} is not supported because it has no base constructor.");
                return;
            }

            il.Add(new CilInstruction(CilOpCodes.Ldarg_0)); // load 'this' for the call
            il.Add(new CilInstruction(CilOpCodes.Ldloc, ctorLocalObj)); // load the new object pointer for the call
            il.Add(new CilInstruction(CilOpCodes.Call, objPointerExplicitFromIntPtr.ToMethodDescriptor(module))); // create the ObjectPointer struct on the stack for the call
            il.Add(new CilInstruction(CilOpCodes.Call, baseCtor.ToMethodDescriptor(module))); // now call the internal ctor
        }

        il.Add(new CilInstruction(CilOpCodes.Ldc_I4_0)); // isGeneric = false
        il.Add(new CilInstruction(CilOpCodes.Ldstr, methodName));
        il.Add(new CilInstruction(CilOpCodes.Ldstr, returnTypeName));

        // build the string array for parameter types
        il.Add(new CilInstruction(CilOpCodes.Ldc_I4, paramTypeNames.Length));
        il.Add(new CilInstruction(CilOpCodes.Newarr,
            module.CorLibTypeFactory.String.ToTypeDefOrRef()));
        for (int i = 0; i < paramTypeNames.Length; i++)
        {
            il.Add(new CilInstruction(CilOpCodes.Dup));
            il.Add(new CilInstruction(CilOpCodes.Ldc_I4, i));
            il.Add(new CilInstruction(CilOpCodes.Ldstr, paramTypeNames[i]));
            il.Add(new CilInstruction(CilOpCodes.Stelem_Ref));
        }

        il.Add(new CilInstruction(CilOpCodes.Call, il2cppGetIl2CppMethod.ToMethodDescriptor(module)));
        // now we have the MethodInfo pointer on the stack

        // ----- 1.5. If we're generic, we need to make the generic method first -----
        if (methodCtx is ConcreteGenericMethodAnalysisContext || methodDef.HasGenericParameters)
        {
            // build the type array
            il.Add(new CilInstruction(CilOpCodes.Ldc_I4, methodDef.GenericParameters.Count));
            il.Add(new CilInstruction(CilOpCodes.Newarr, appContext.SystemTypes.SystemTypeType.ToTypeSignature(module).ToTypeDefOrRef()));

            for (int i = 0; i < methodDef.GenericParameters.Count; i++)
            {
                var param = methodDef.GenericParameters[i];

                // arr[i] = typeof(T)
                il.Add(new CilInstruction(CilOpCodes.Dup));
                il.Add(new CilInstruction(CilOpCodes.Ldc_I4, i));

                // refer to the generic parameter as !!i (method generic param by index)
                var genericParamSig = new GenericParameterSignature(GenericParameterType.Method, i);
                var typeSpec = new TypeSpecification(genericParamSig);
                var importedTypeSpec = module.DefaultImporter.ImportType(typeSpec);

                il.Add(new CilInstruction(CilOpCodes.Ldtoken, importedTypeSpec));
                il.Add(new CilInstruction(CilOpCodes.Call, systemTypeGetFromHandle.ToMethodDescriptor(module)));
                il.Add(new CilInstruction(CilOpCodes.Stelem_Ref));
            }

            // make the generic method
            il.Add(new CilInstruction(CilOpCodes.Call, il2cppMakeGenericMethod.ToMethodDescriptor(module)));
        }

        // ----- 2. Build the void** args array via IL2CPP.InvokeMethod -----
        // IL2CPP.InvokeMethod(IntPtr method, IntPtr instance, object?[] args) → object?

        // methodPtr already on stack, store in local so we can reuse.
        var localMethod = new CilLocalVariable(module.CorLibTypeFactory.IntPtr);
        methodDef.CilMethodBody.LocalVariables.Add(localMethod);
        il.Add(new CilInstruction(CilOpCodes.Stloc, localMethod));

        // push back the method pointer for the call
        il.Add(new CilInstruction(CilOpCodes.Ldloc, localMethod));

        if (isStatic)
        {
            // IntPtr.Zero
            il.Add(new CilInstruction(CilOpCodes.Ldc_I4_0));
            il.Add(new CilInstruction(CilOpCodes.Conv_I));
        }
        else
        {
            // Il2CppObject.Pointer (casted to IntPtr)
            il.Add(new CilInstruction(CilOpCodes.Ldarg_0));
            il.Add(new CilInstruction(CilOpCodes.Callvirt, il2cppObjectGetPointer.ToMethodDescriptor(module)));
            il.Add(new CilInstruction(CilOpCodes.Call, objPointerExplicitIntPtr.ToMethodDescriptor(module)));
        }

        // Args array: box all parameters.
        // For a static method params start at arg 0, for instance at arg 1.
        int argOffset = isStatic ? 0 : 1;
        int paramCount = methodDef.Parameters.Count;

        il.Add(new CilInstruction(CilOpCodes.Ldc_I4, paramCount));
        il.Add(new CilInstruction(CilOpCodes.Newarr, module.CorLibTypeFactory.Object.ToTypeDefOrRef()));

        for (int i = 0; i < paramCount; i++)
        {
            var param = methodDef.Parameters[i];
            il.Add(new CilInstruction(CilOpCodes.Dup));
            il.Add(new CilInstruction(CilOpCodes.Ldc_I4, i));
            il.Add(new CilInstruction(CilOpCodes.Ldarg, param));
            // Box value types so they fit in object[].
            if (param.ParameterType is not null && IsValueOrPrimitive(param.ParameterType))
                il.Add(new CilInstruction(CilOpCodes.Box, param.ParameterType.ToTypeDefOrRef()));
            il.Add(new CilInstruction(CilOpCodes.Stelem_Ref));
        }

        il.Add(new CilInstruction(CilOpCodes.Call, il2cppInvokeMethod.ToMethodDescriptor(module)));
        // object? result is now on the stack

        // ----- 3. Handle return value -----
        if (isVoid)
        {
            il.Add(new CilInstruction(CilOpCodes.Pop));
        }
        else
        {
            var returnType = methodDef.Signature!.ReturnType;
            if (IsValueOrPrimitive(returnType))
            {
                // Unbox value type result.
                il.Add(new CilInstruction(CilOpCodes.Unbox_Any, returnType.ToTypeDefOrRef()));
            }
            else if (returnType.FullName == "System.String")
            {
                // Cast reference type result (string) to correct type.
                // The conversion from IL2CPP to Managed is handled by UnboxResult inside IL2CPP.InvokeMethod
                il.Add(new CilInstruction(CilOpCodes.Castclass, returnType.ToTypeDefOrRef()));
            }
            else
            {
                // cast the object? to IntPtr
                il.Add(new CilInstruction(CilOpCodes.Unbox_Any, module.CorLibTypeFactory.IntPtr.ToTypeDefOrRef()));
                // call Il2CppObject.Wrap<T>() on the pointer
                il.Add(new CilInstruction(CilOpCodes.Call, il2cppObjectWrap.ToMethodDescriptor(module)));
            }
        }

        il.Add(new CilInstruction(CilOpCodes.Ret));
        il.OptimizeMacros();
    }

    private static void EmitNotSupported(CilInstructionCollection il, ModuleDefinition module, string message)
    {
        var notSupportedCtor = module.CorLibTypeFactory.CorLibScope
            .CreateTypeReference("System", "NotSupportedException")
            .CreateMemberReference(".ctor",
                MethodSignature.CreateInstance(module.CorLibTypeFactory.Void,
                    module.CorLibTypeFactory.String))
            .ImportWith(module.DefaultImporter);

        il.Add(new CilInstruction(CilOpCodes.Ldstr, message));
        il.Add(new CilInstruction(CilOpCodes.Newobj, notSupportedCtor));
        il.Add(new CilInstruction(CilOpCodes.Throw));
    }

    /// <summary>
    /// Returns true for types that need box/unbox rather than castclass.
    /// </summary>
    private static bool IsValueOrPrimitive(TypeSignature sig) =>
        sig is CorLibTypeSignature { ElementType: not (ElementType.String or ElementType.Object) }
        || sig is TypeDefOrRefSignature { Type.IsValueType: true };

    /// <summary>
    /// Gets a flattened il2cpp type name string matching what GetIl2CppMethod expects.
    /// Strips generic arity markers (`1, `2 etc.) to match the IL2CPP runtime name format.
    /// </summary>
    private static string GetTypeName(TypeAnalysisContext? ctx)
    {
        if (ctx == null) return "System.Void";
        // Use the full dotnet name, stripping generic backtick arity to match IL2CPP naming.
        var name = ctx.DefaultName ?? "System.Object";
        // Replace nested type separator and generic arity.
        name = System.Text.RegularExpressions.Regex.Replace(name, @"`\d+", "");
        name = name.Replace('/', '.').Replace('+', '.');
        return name;
    }

    public override List<AssemblyDefinition> BuildAssemblies(ApplicationAnalysisContext context)
    {
        var list = base.BuildAssemblies(context);

        var referenceAssemblies = context.Assemblies.Where(a => a.IsReferenceAssembly).Select(a => a.Name).ToHashSet();

        // Remove injected reference assemblies from the output
        for (var i = list.Count - 1; i >= 0; i--)
        {
            if (referenceAssemblies.Contains(list[i].Name ?? ""))
                list.RemoveAt(i);
        }

        // Replace mscorlib references with .NET Core references
        var dotNetCorLib = KnownCorLibs.SystemRuntime_v10_0_0_0;
        foreach (var assembly in list)
        {
            foreach (var module in assembly.Modules)
            {
                foreach (var reference in module.AssemblyReferences)
                {
                    if (reference.Name == "mscorlib")
                    {
                        reference.Name = dotNetCorLib.Name;
                        reference.Version = dotNetCorLib.Version;
                        reference.Attributes = dotNetCorLib.Attributes;
                        reference.PublicKeyOrToken = dotNetCorLib.PublicKeyOrToken;
                        reference.HashValue = dotNetCorLib.HashValue;
                        reference.Culture = dotNetCorLib.Culture;
                    }
                }
            }
        }

        return list;
    }
}