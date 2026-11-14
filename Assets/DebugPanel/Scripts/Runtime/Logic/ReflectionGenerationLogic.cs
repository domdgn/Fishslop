#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DebugPanel.Attributes;
using DebugPanel.Constants;
using DebugPanel.Datas;
using DebugPanel.DebugActions.Containers;
using DebugPanel.Extensions;

namespace DebugPanel.Logic
{
    public sealed class ReflectionGenerationLogic
    {
        readonly DebugActionsSectionsLogic _debugActionsSectionsLogic;
        readonly ReflectionData _reflectionData;
        readonly FrameSlicingData _frameSlicingData;

        public ReflectionGenerationLogic(
            DebugActionsSectionsLogic debugActionsSectionsLogic, 
            ReflectionData reflectionData, 
            FrameSlicingData frameSlicingData
            )
        {
            _debugActionsSectionsLogic = debugActionsSectionsLogic;
            _reflectionData = reflectionData;
            _frameSlicingData = frameSlicingData;
        }

        public void CreateDebugActionForPropertyInfo(
            DebugActionsSection section, 
            object? optionsObject,
            PropertyInfo propertyInfo,
            MemberCreationData memberCreationData
            )
        {
            MethodInfo? getter = propertyInfo.GetGetMethod(true);
            MethodInfo? setter = propertyInfo.GetSetMethod(true);
            
            bool hasPublicGetter = getter != null && getter.IsPublic;
            bool hasPublicSetter = setter != null && setter.IsPublic;
            
            if (getter != null && memberCreationData.ShowAsDebugAction)
            {
                hasPublicGetter = true;
            }

            if (setter != null && memberCreationData.ShowAsDebugAction)
            {
                hasPublicSetter = true;
            }

            if (memberCreationData.DontShowAsDebugAction)
            {
                hasPublicGetter = false;
                hasPublicSetter = false;
            }
            
            bool hasPublicGetterAndSetter = hasPublicGetter && hasPublicSetter;

            if (hasPublicGetterAndSetter)
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    string displayName = GetDisplayNameFromPropertyInfo(propertyInfo);
                    
                    section.AddButtonStringSelector(
                        displayName,
                        val => propertyInfo.SetValue(optionsObject, val),
                        () => (string)propertyInfo.GetValue(optionsObject)!
                    );
                }
                else if (propertyInfo.PropertyType == typeof(bool))
                {
                    string displayName = GetDisplayNameFromPropertyInfo(propertyInfo);

                    section.AddToggle(
                        displayName,
                        val => propertyInfo.SetValue(optionsObject, val),
                        () => (bool)propertyInfo.GetValue(optionsObject)!
                    );
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    string displayName = GetDisplayNameFromPropertyInfo(propertyInfo);
                    float step = GetStepFromPropertyInfo(propertyInfo);
                    (float min, float max) range = GetRangeFromPropertyInfo(propertyInfo);

                    section.AddIntSelector(
                        displayName,
                        (int)step,
                        val =>
                        {
                            val = (int)Math.Clamp(val, range.min, range.max);
                            propertyInfo.SetValue(optionsObject, val);
                        },
                        () => (int)propertyInfo.GetValue(optionsObject)!
                    );
                }
                else if (propertyInfo.PropertyType == typeof(float))
                {
                    string displayName = GetDisplayNameFromPropertyInfo(propertyInfo);
                    float step = GetStepFromPropertyInfo(propertyInfo);
                    (float min, float max) range = GetRangeFromPropertyInfo(propertyInfo);
                    
                    section.AddFloatSelector(
                        displayName,
                        step,
                        val =>
                        {
                            val = Math.Clamp(val, range.min, range.max);
                            propertyInfo.SetValue(optionsObject, val);
                        },
                        () => (float)propertyInfo.GetValue(optionsObject)!
                    );
                }
                else if (propertyInfo.PropertyType == typeof(long))
                {
                    string displayName = GetDisplayNameFromPropertyInfo(propertyInfo);
                    float step = GetStepFromPropertyInfo(propertyInfo);
                    (float min, float max) range = GetRangeFromPropertyInfo(propertyInfo);
                    
                    section.AddLongSelector(
                        displayName,
                        (long)step,
                        val =>
                        {
                            val = (long)Math.Clamp(val, range.min, range.max);
                            propertyInfo.SetValue(optionsObject, val);
                        },
                        () => (long)propertyInfo.GetValue(optionsObject)!
                    );
                }
                else if(typeof(Enum).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    string displayName = GetDisplayNameFromPropertyInfo(propertyInfo);
                    
                    section.AddButtonEnumSelector(
                        displayName,
                        propertyInfo.PropertyType,
                        val => propertyInfo.SetValue(optionsObject, val),
                        () => (Enum)propertyInfo.GetValue(optionsObject)!
                    );
                }

                return;
            }

            if (hasPublicGetter)
            {
                string displayName = GetDisplayNameFromPropertyInfo(propertyInfo);
                
                section.AddInfoDynamic(() =>
                {
                    object value = propertyInfo.GetValue(optionsObject);
                    string valueString = value != null ? value.ToString() : "Null";
                    
                    return $"{displayName}: {valueString}";
                });

                return;
            }
        }

        public void CreateDebugActionForMethodInfo(
            DebugActionsSection section, 
            object? optionsObject, 
            MethodInfo methodInfo,
            MemberCreationData memberCreationData
            )
        {
            bool isValid = methodInfo.GetParameters().Length == 0
                           && !methodInfo.IsSpecialName
                           && methodInfo.DeclaringType != typeof(object);

            if (!isValid)
            {
                return;
            }

            if (methodInfo.IsPrivate && !memberCreationData.ShowAsDebugAction)
            {
                return;
            }

            if (methodInfo.IsPublic && memberCreationData.DontShowAsDebugAction)
            {
                return;
            }
        
            section.AddButton(
                methodInfo.Name,
                () => methodInfo.Invoke(optionsObject, Array.Empty<object>())
            );
        }
        
        void CreateDebugActionForMemberInfo(DebugActionsSection section, object? optionsObject, MemberInfo memberInfo)
        {
            bool showAsDebugActionAttribute = memberInfo.GetCustomAttribute<ShowAsDebugActionAttribute>() != null;
            bool dontShowAsDebugActionAttribute = memberInfo.GetCustomAttribute<DontShowAsDebugActionAttribute>() != null;

            MemberCreationData memberCreationData = new(
                showAsDebugActionAttribute,
                dontShowAsDebugActionAttribute
            );
            
            if (memberInfo is PropertyInfo propertyInfo)
            {
                CreateDebugActionForPropertyInfo(section, optionsObject, propertyInfo, memberCreationData);
            }
            else if (memberInfo is MethodInfo methodInfo)
            {
                CreateDebugActionForMethodInfo(section, optionsObject, methodInfo, memberCreationData);
            }
        }

        public void PopulateDebugActionsFromGlobalOptionsObjects()
        {
            if (_reflectionData.GlobalOptionsActionsCreated)
            {
                return;
            }

            _reflectionData.GlobalOptionsActionsCreated = true;
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    TryPopulateDebugActionsAsGlobalOptionsType(type);
                }
            }
        }
        
        public async Task PopulateDebugActionsFromGlobalOptionsObjectsAsync(CancellationToken cancellationToken)
        {
            if (_reflectionData.GlobalOptionsActionsCreated)
            {
                return;
            }

            _reflectionData.GlobalOptionsActionsCreated = true;
            
            Stopwatch frameTimeStopwatch = Stopwatch.StartNew();
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    if(cancellationToken.IsCancellationRequested) return;
                    
                    TryPopulateDebugActionsAsGlobalOptionsType(type);
                    
                    if (frameTimeStopwatch.ElapsedMilliseconds > _frameSlicingData.MaxMsPerFrame)
                    {
                        await Task.Yield();

                        frameTimeStopwatch.Restart();
                    }
                }
            }
        }

        public void TryPopulateDebugActionsAsGlobalOptionsType(Type type)
        {
            bool isValidType = type.IsClass || type is { IsValueType: true, IsEnum: false };

            if (!isValidType)
            {
                return;
            }

            bool isStatic = type is { IsAbstract: true, IsSealed: true };

            if (!isStatic)
            {
                return;
            }

            DebugActionsObject? debugActionsObjectAttribute = type.GetCustomAttribute<DebugActionsObject>();

            if (debugActionsObjectAttribute == null)
            {
                return;
            }
            
            PopulateDebugActionsFromStaticOptionsObjectType(type);
        }

        public void PopulateDebugActionsSectionFromOptionsObject(DebugActionsSection section, object optionsObject)
        {
            Type type = optionsObject.GetType();

            IEnumerable<MemberInfo> members = GetMembersInfoFromType(type);

            foreach (MemberInfo memberInfo in members)
            {
                CreateDebugActionForMemberInfo(section, optionsObject, memberInfo);
            }
        }

        public void PopulateDebugActionsFromOptionsObject(object optionsObject)
        {
            Type type = optionsObject.GetType();

            IEnumerable<MemberInfo> members = GetMembersInfoFromType(type);

            foreach (MemberInfo memberInfo in members)
            {
                DebugActionsSection section = GetSectionForMemeberInfo(type, memberInfo);
                CreateDebugActionForMemberInfo(section, optionsObject, memberInfo);
            }
        }
        
        public void PopulateDebugActionsFromStaticOptionsObjectType(Type optionsObjectType)
        {
            IEnumerable<MemberInfo> members = GetMembersInfoFromType(optionsObjectType);

            foreach (MemberInfo memberInfo in members)
            {
                DebugActionsSection section = GetSectionForMemeberInfo(optionsObjectType, memberInfo);
                CreateDebugActionForMemberInfo(section, null, memberInfo);
            }
        }

        public IEnumerable<MemberInfo> GetMembersInfoFromType(Type type)
        {
            return type
                .GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .OrderBy(member => member.MetadataToken);
        }

        public DebugActionsSection GetSectionForMemeberInfo(Type ownerType, MemberInfo memberInfo)
        {
            CategoryAttribute? categoryAttribute = memberInfo.GetCustomAttribute<CategoryAttribute>();

            string sectionName = categoryAttribute != null ? categoryAttribute.Category : ownerType.Name;

            return _debugActionsSectionsLogic.GetOrAddDebugActionsSection(sectionName, 0);
        }
        
        public string GetDisplayNameFromPropertyInfo(PropertyInfo propertyInfo)
        {
            DisplayNameAttribute? displayNameAttribute = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
            return displayNameAttribute != null ? displayNameAttribute.DisplayName : propertyInfo.Name;
        }

        public float GetStepFromPropertyInfo(PropertyInfo propertyInfo)
        {
            float step = 1f;
                    
            NumberStepAttribute? stepAttribute = propertyInfo.GetCustomAttribute<NumberStepAttribute>();

            if (stepAttribute != null)
            {
                step = stepAttribute.Step;
            }

            return step;
        }
        
        public (float min, float max) GetRangeFromPropertyInfo(PropertyInfo propertyInfo)
        {
            float min = float.MinValue;
            float max = float.MaxValue;
                    
            NumberRangeAttribute? rangeAttribute = propertyInfo.GetCustomAttribute<NumberRangeAttribute>();

            if (rangeAttribute != null)
            {
                min = rangeAttribute.Min;
                max = rangeAttribute.Max;
            }

            return (min, max);
        }
    }
}