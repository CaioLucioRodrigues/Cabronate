using Cabronate.Base;
using Cabronate.DAO.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Cabronate.DAO.Mount
{
    public class EcalcValueObjectMapper
    {
        public static EcalcValueObjectTableInfo BuildEcalcValueObjectMapper(EcalcValueObject vo)
        {
            Type currentType = vo.GetType();
            EcalcValueObjectTableInfo currentTableInfo;
            if (!EcalcValueObject.tableInfoList.TryGetValue(currentType, out currentTableInfo))
            {
                lock (EcalcValueObject.tableInfoList)
                {
                    // Checar novamente para garantir que não a instância não foi "resgatada" antes da aquisição do lock
                    if (!EcalcValueObject.tableInfoList.TryGetValue(currentType, out currentTableInfo))
                    {
                        currentTableInfo = new EcalcValueObjectTableInfo();

                        PropertyInfo[] Properties = vo.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                        foreach (PropertyInfo property in Properties)
                        {
                            object[] attributes = property.GetCustomAttributes(typeof(FieldNameAttribute), false);
                            if (attributes.Count() > 0)
                            {
                                try
                                {
                                    MountObjectMapperField field = new MountObjectMapperField();
                                    string LocalFieldName = ((FieldNameAttribute)attributes[0]).Description;
                                    bool LocalTemDefault = (property.GetCustomAttributes(typeof(DefaultAttribute), false).Count() > 0);
                                    currentTableInfo.Fields.Add(
                                        new EcalcValueObjectFieldInfo()
                                        {
                                            Property = property,
                                            FieldName = LocalFieldName,
                                            TemDefault = LocalTemDefault
                                        });

                                }
                                catch (IndexOutOfRangeException)
                                {
                                    continue;
                                }
                            }
                        }
                        EcalcValueObject.tableInfoList.Add(currentType, currentTableInfo);
                    }
                }
            }

            return currentTableInfo;
        }
    }
}
