using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;

namespace Fanex.Customer.API.Tests.Modules.CustomerSetting.Shared.Services;

internal static class SettingProfileTestHelper
{
    internal static List<SettingProfile> CreateSampleSettingProfiles()
    {
        return new List<SettingProfile>
        {
            new SettingProfile
            {
                SportType = 1,
                BetType = 1,
                IsLive = false,
                GroupId = 1,
                GroupType = 150,
                SubGroupId = null,
                DisplayType = 1,
                SettingLevel = 1,
                SettingType = 10,
                DisplayMode = 1,
                ProductId = 1
            },
            new SettingProfile
            {
                SportType = 1,
                BetType = 2,
                IsLive = true,
                GroupId = 1,
                GroupType = 151,
                SubGroupId = 1,
                DisplayType = 2,
                SettingLevel = 2,
                SettingType = 20,
                DisplayMode = 1,
                ProductId = 2
            },
            new SettingProfile
            {
                SportType = 2,
                BetType = 1,
                IsLive = false,
                GroupId = 2,
                GroupType = 152,
                SubGroupId = null,
                DisplayType = 1,
                SettingLevel = 1,
                SettingType = 30,
                DisplayMode = 2,
                ProductId = 3
            },
            new SettingProfile
            {
                SportType = 2,
                BetType = 2,
                IsLive = true,
                GroupId = 2,
                GroupType = 153,
                SubGroupId = 2,
                DisplayType = 2,
                SettingLevel = 3,
                SettingType = 40,
                DisplayMode = 1,
                ProductId = 4
            }
        };
    }

    internal static SettingProfile CreateSettingProfile(int groupType, int settingType)
    {
        return new SettingProfile
        {
            SportType = 1,
            BetType = 1,
            IsLive = false,
            GroupId = 1,
            GroupType = groupType,
            SubGroupId = null,
            DisplayType = 1,
            SettingLevel = 1,
            SettingType = settingType,
            DisplayMode = 1,
            ProductId = 1
        };
    }

    internal static List<SettingProfile> CreateSettingProfilesWithDuplicateSettingTypes()
    {
        return new List<SettingProfile>
        {
            CreateSettingProfile(150, 10),
            CreateSettingProfile(151, 20),
            CreateSettingProfile(152, 10), // Duplicate SettingType
            CreateSettingProfile(153, 30),
            CreateSettingProfile(154, 20), // Another duplicate
            CreateSettingProfile(155, 40)
        };
    }
}
