﻿using System;
using System.Runtime.CompilerServices;

namespace SiliconStudio.Xenko.Updater
{
    /// <summary>
    /// Base class for <see cref="UpdatableListAccessor{T}"/>.
    /// </summary>
    abstract class UpdatableListAccessor : UpdatableCustomAccessor
    {
        public readonly int Index;

        protected UpdatableListAccessor(int index)
        {
            Index = index;
        }
    }

    /// <summary>
    /// Describes how to get or set a list value for the <see cref="UpdateEngine"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class UpdatableListAccessor<T> : UpdatableListAccessor
    {
        public UpdatableListAccessor(int index) : base(index)
        {
        }

        /// <inheritdoc/>
        public override Type MemberType
        {
            get { return typeof(T); }
        }

        /// <inheritdoc/>
        public override IntPtr GetStructAndUnbox(IntPtr obj, object data)
        {
#if IL
            // Note: IL is injected by UpdateEngineProcessor
            ldarg data
            unbox !T
            dup
            ldarg obj
            ldarg.0
            ldfld int32 SiliconStudio.Xenko.Updater.UpdatableListAccessor::Index
            callvirt instance !T class [mscorlib]System.Collections.Generic.IList`1<!T>::get_Item(int32)
            stobj !T
            ret
#endif
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void GetBlittable(IntPtr obj, IntPtr data)
        {
#if IL
            // Note: IL is injected by UpdateEngineProcessor
            ldarg data
            ldarg obj
            ldarg.0
            ldfld int32 SiliconStudio.Xenko.Updater.UpdatableListAccessor::Index
            callvirt instance !T class [mscorlib]System.Collections.Generic.IList`1<!T>::get_Item(int32)
            stobj !T
            ret
#endif
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void SetStruct(IntPtr obj, object data)
        {
#if IL
            // Note: IL is injected by UpdateEngineProcessor
            ldarg obj
            ldarg.0
            ldfld int32 SiliconStudio.Xenko.Updater.UpdatableListAccessor::Index
            ldarg data
            unbox.any !T
            callvirt instance void class [mscorlib]System.Collections.Generic.IList`1<!T>::set_Item(int32, !0)
            ret
#endif
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void SetBlittable(IntPtr obj, IntPtr data)
        {
#if IL
            // Note: IL is injected by UpdateEngineProcessor
            ldarg obj
            ldarg.0
            ldfld int32 SiliconStudio.Xenko.Updater.UpdatableListAccessor::Index
            ldarg data
            ldobj !T
            callvirt instance void class [mscorlib]System.Collections.Generic.IList`1<!T>::set_Item(int32, !0)
            ret
#endif
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override object GetObject(IntPtr obj)
        {
#if IL
            // Use method to set testI
            ldarg obj
            ldarg.0
            ldfld int32 SiliconStudio.Xenko.Updater.UpdatableListAccessor::Index
            callvirt instance !T class [mscorlib]System.Collections.Generic.IList`1<!T>::get_Item(int32)
            ret
#endif
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void SetObject(IntPtr obj, object data)
        {
#if IL
            // Note: IL is injected by UpdateEngineProcessor
            ldarg obj
            ldarg.0
            ldfld int32 SiliconStudio.Xenko.Updater.UpdatableListAccessor::Index
            ldarg data
            callvirt instance void class [mscorlib]System.Collections.Generic.IList`1<!T>::set_Item(int32, !0)
            ret
#endif
            throw new NotImplementedException();
        }
    }
}