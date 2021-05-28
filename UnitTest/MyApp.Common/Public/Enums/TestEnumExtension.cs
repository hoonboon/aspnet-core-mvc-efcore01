using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace MyApp.Common.Public.Enums
{
    public class TestEnumExtension
    {
        public static TheoryData<TestEnums, EnumItemDisplayValues> TestData1 => 
            new TheoryData<TestEnums, EnumItemDisplayValues>
            {
                {
                    TestEnums.TestVal1,
                    new EnumItemDisplayValues {
                        GroupName = "GroupVal",
                        Name = "NameVal",
                        Description = "DescriptionVal"
                    }
                },
                {
                    TestEnums.TestVal2,
                    new EnumItemDisplayValues {
                        Name = "NameVal"
                    }
                }
            };

        [Theory]
        [MemberData(nameof(TestData1))]
        public void TestGetEnumItemDisplayValues(TestEnums testValue, EnumItemDisplayValues expected)
        {
            var result = testValue.GetEnumItemDisplayValues();
            
            Assert.Equal(expected.GroupName, result.GroupName);
            Assert.Equal(expected.Name, result.Name);
            Assert.Equal(expected.Description, result.Description);
            Assert.Equal(expected.IsObsolete, result.IsObsolete);

        }
    }

    public enum TestEnums : short
    {
        [Display(GroupName = "GroupVal", Name = "NameVal", Description = "DescriptionVal")]
        TestVal1 = 10, 
        [Display(Name = "NameVal")]
        TestVal2 = 20,
    }
}
