using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Data.Models.Housing
{
    public static class EnumHelper
    {
        public static Dictionary<string, List<EnumValueDto>> GetAllEnums()
        {
            var enumDictionary = new Dictionary<string, List<EnumValueDto>>();

            // Get all enums in the Housing namespace
            var enums = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsEnum && t.Namespace == "StudentPath.DAL.Data.Models.Housing");

            foreach (var enumType in enums)
            {
                var enumValues = Enum.GetValues(enumType)
                    .Cast<Enum>()
                    .Select(e => new EnumValueDto
                    {
                        Name = e.ToString(),
                        Value = Convert.ToInt32(e)
                    })
                    .ToList();

                enumDictionary.Add(enumType.Name, enumValues);
            }

            return enumDictionary;
        }
    }

    public class EnumValueDto
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
