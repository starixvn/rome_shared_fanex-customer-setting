using Fanex.Caching;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.Services;
using Fanex.Customer.API.Modules.CustomerSetting.Shared.ViewModels;
using Fanex.Customer.API.Modules.QueryCustomer.Criteria;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using Moq;
using static Fanex.Customer.API.Modules.CustomerSetting.Shared.Services.SettingProfileService;

namespace Fanex.Customer.API.Tests.Modules.CustomerSetting.Shared.Services;

public class SettingProfileServiceTests
{
    private readonly Mock<IDynamicRepository> _mockDynamicRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IOptions<AppSettings>> _mockAppSettings;
    private readonly AppSettings _appSettings;
    private readonly SettingProfileService _settingProfileService;

    public SettingProfileServiceTests()
    {
        _mockDynamicRepository = new Mock<IDynamicRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _mockAppSettings = new Mock<IOptions<AppSettings>>();
        
        _appSettings = new AppSettings
        {
            SettingStatus = 1
        };
        
        _mockAppSettings.Setup(x => x.Value).Returns(_appSettings);
        
        _settingProfileService = new SettingProfileService(
            _mockDynamicRepository.Object,
            _mockAppSettings.Object,
            _mockCacheService.Object
        );
    }

    #region GetSettingProfiles Tests

    [Fact]
    public async Task GetSettingProfiles_ShouldReturnCachedData_WhenCacheHit()
    {
        // Arrange
        byte systemId = 1;
        string site = "TEST";
        int currency = 840;
        var mode = ProfileMode.AddNew;
        var serviceType = ServiceType.Credit;

        var expectedProfiles = SettingProfileTestHelper.CreateSampleSettingProfiles().Take(2).ToList();

        var expectedCacheKey = $"CreateCustomerSettingProfile_{systemId}_{site}_{currency}_{mode}_{serviceType}";
        
        _mockCacheService
            .Setup(x => x.GetOrSetAsync(
                expectedCacheKey,
                It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
                It.IsAny<CacheItemOptions>()))
            .ReturnsAsync(expectedProfiles);

        // Act
        var result = await _settingProfileService.GetSettingProfiles(systemId, site, currency, mode, serviceType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(10, expectedProfiles.First().SettingType);
        Assert.Equal(20, expectedProfiles.Last().SettingType);

        _mockCacheService.Verify(x => x.GetOrSetAsync(
            expectedCacheKey,
            It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
            It.IsAny<CacheItemOptions>()), Times.Once);
    }

    [Fact]
    public async Task GetSettingProfiles_ShouldUseDefaultServiceType_WhenNotSpecified()
    {
        // Arrange
        byte systemId = 1;
        string site = "TEST";
        int currency = 840;
        var mode = ProfileMode.AddNew;

        var expectedCacheKey = $"CreateCustomerSettingProfile_{systemId}_{site}_{currency}_{mode}_{ServiceType.Credit}";

        _mockCacheService
            .Setup(x => x.GetOrSetAsync(
                expectedCacheKey,
                It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
                It.IsAny<CacheItemOptions>()))
            .ReturnsAsync(new List<SettingProfile>());

        // Act
        await _settingProfileService.GetSettingProfiles(systemId, site, currency, mode);

        // Assert
        _mockCacheService.Verify(x => x.GetOrSetAsync(
            expectedCacheKey,
            It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
            It.IsAny<CacheItemOptions>()), Times.Once);
    }

    [Theory]
    [InlineData(ProfileMode.AddNew, ServiceType.Credit)]
    [InlineData(ProfileMode.Edit, ServiceType.Cash)]
    [InlineData(ProfileMode.DirectMember, ServiceType.Credit)]
    public async Task GetSettingProfiles_ShouldGenerateCorrectCacheKey_ForDifferentModeAndServiceType(
        ProfileMode mode, ServiceType serviceType)
    {
        // Arrange
        byte systemId = 1;
        string site = "TEST";
        int currency = 840;

        var expectedCacheKey = $"CreateCustomerSettingProfile_{systemId}_{site}_{currency}_{mode}_{serviceType}";

        _mockCacheService
            .Setup(x => x.GetOrSetAsync(
                expectedCacheKey,
                It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
                It.IsAny<CacheItemOptions>()))
            .ReturnsAsync(new List<SettingProfile>());

        // Act
        await _settingProfileService.GetSettingProfiles(systemId, site, currency, mode, serviceType);

        // Assert
        _mockCacheService.Verify(x => x.GetOrSetAsync(
            expectedCacheKey,
            It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
            It.IsAny<CacheItemOptions>()), Times.Once);
    }

    #endregion

    #region GetSettingTypeByGroupType Tests

    [Fact]
    public async Task GetSettingTypeByGroupType_ShouldReturnCorrectSettingType_WhenGroupTypeExists()
    {
        // Arrange
        byte systemId = 1;
        string site = "TEST";
        int currency = 840;
        int groupTypeId = 150;
        var mode = ProfileMode.AddNew;
        var serviceType = ServiceType.Credit;

        var settingProfiles = SettingProfileTestHelper.CreateSampleSettingProfiles().Take(2).ToList();

        var cacheKey = $"CreateCustomerSettingProfile_{systemId}_{site}_{currency}_{mode}_{serviceType}";
        
        _mockCacheService
            .Setup(x => x.GetOrSetAsync(
                cacheKey,
                It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
                It.IsAny<CacheItemOptions>()))
            .ReturnsAsync(settingProfiles);

        // Act
        var result = await _settingProfileService.GetSettingTypeByGroupType(
            systemId, site, currency, groupTypeId, mode, serviceType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Value);
    }

    [Fact]
    public async Task GetSettingTypeByGroupType_ShouldReturnNull_WhenGroupTypeNotFound()
    {
        // Arrange
        byte systemId = 1;
        string site = "TEST";
        int currency = 840;
        int groupTypeId = 999; // Non-existent group type
        var mode = ProfileMode.AddNew;
        var serviceType = ServiceType.Credit;

        var settingProfiles = SettingProfileTestHelper.CreateSampleSettingProfiles().Take(2).ToList();

        var cacheKey = $"CreateCustomerSettingProfile_{systemId}_{site}_{currency}_{mode}_{serviceType}";
        
        _mockCacheService
            .Setup(x => x.GetOrSetAsync(
                cacheKey,
                It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
                It.IsAny<CacheItemOptions>()))
            .ReturnsAsync(settingProfiles);

        // Act
        var result = await _settingProfileService.GetSettingTypeByGroupType(
            systemId, site, currency, groupTypeId, mode, serviceType);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetSettingTypeByGroupType_ShouldUseDefaultServiceType_WhenNotSpecified()
    {
        // Arrange
        byte systemId = 1;
        string site = "TEST";
        int currency = 840;
        int groupTypeId = 150;
        var mode = ProfileMode.AddNew;

        var settingProfiles = SettingProfileTestHelper.CreateSampleSettingProfiles().Where(p => p.GroupType == 150).ToList();

        var expectedCacheKey = $"CreateCustomerSettingProfile_{systemId}_{site}_{currency}_{mode}_{ServiceType.Credit}";
        
        _mockCacheService
            .Setup(x => x.GetOrSetAsync(
                expectedCacheKey,
                It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
                It.IsAny<CacheItemOptions>()))
            .ReturnsAsync(settingProfiles);

        // Act
        var result = await _settingProfileService.GetSettingTypeByGroupType(
            systemId, site, currency, groupTypeId, mode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Value);
    }

    #endregion

    #region GetAllSettingTypes Tests

    [Fact]
    public async Task GetAllSettingTypes_ShouldReturnDistinctSettingTypes()
    {
        // Arrange
        byte systemId = 1;
        string site = "TEST";
        int currency = 840;
        var mode = ProfileMode.AddNew;
        var serviceType = ServiceType.Credit;

        var settingProfiles = SettingProfileTestHelper.CreateSettingProfilesWithDuplicateSettingTypes();

        _mockDynamicRepository
            .Setup(x => x.FetchAsync<SettingProfile>(It.IsAny<GetSettingProfileCriteria>()))
            .ReturnsAsync(settingProfiles);

        // Act
        var result = await _settingProfileService.GetAllSettingTypes(systemId, site, currency, mode, serviceType);

        // Assert
        Assert.NotNull(result);
        var settingTypesList = result.ToList();
        Assert.Equal(4, settingTypesList.Count); // Should be distinct: 10, 20, 30, 40
        Assert.Contains(10, settingTypesList);
        Assert.Contains(20, settingTypesList);
        Assert.Contains(30, settingTypesList);
        Assert.Contains(40, settingTypesList);

        _mockDynamicRepository.Verify(x => x.FetchAsync<SettingProfile>(
            It.Is<GetSettingProfileCriteria>(c => 
                c.SettingStatus == _appSettings.SettingStatus &&
                c.Site == site &&
                c.Currency == currency &&
                c.SystemId == systemId &&
                c.Mode == (byte)mode &&
                c.ServiceType == (byte)serviceType)), Times.Once);
    }

    [Fact]
    public async Task GetAllSettingTypes_ShouldReturnEmptyCollection_WhenNoProfilesFound()
    {
        // Arrange
        byte systemId = 1;
        string site = "TEST";
        int currency = 840;
        var mode = ProfileMode.AddNew;
        var serviceType = ServiceType.Credit;

        _mockDynamicRepository
            .Setup(x => x.FetchAsync<SettingProfile>(It.IsAny<GetSettingProfileCriteria>()))
            .ReturnsAsync(new List<SettingProfile>());

        // Act
        var result = await _settingProfileService.GetAllSettingTypes(systemId, site, currency, mode, serviceType);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllSettingTypes_ShouldUseDefaultServiceType_WhenNotSpecified()
    {
        // Arrange
        byte systemId = 1;
        string site = "TEST";
        int currency = 840;
        var mode = ProfileMode.AddNew;

        _mockDynamicRepository
            .Setup(x => x.FetchAsync<SettingProfile>(It.IsAny<GetSettingProfileCriteria>()))
            .ReturnsAsync(new List<SettingProfile>());

        // Act
        await _settingProfileService.GetAllSettingTypes(systemId, site, currency, mode);

        // Assert
        _mockDynamicRepository.Verify(x => x.FetchAsync<SettingProfile>(
            It.Is<GetSettingProfileCriteria>(c => 
                c.ServiceType == (byte)ServiceType.Credit)), Times.Once);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldCreateInstance_WithValidDependencies()
    {
        // Arrange & Act
        var service = new SettingProfileService(
            _mockDynamicRepository.Object,
            _mockAppSettings.Object,
            _mockCacheService.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_ShouldUseDefaultRepository_WhenRepositoryIsNull()
    {
        // Arrange & Act
        var service = new SettingProfileService(
            null,
            _mockAppSettings.Object,
            _mockCacheService.Object);

        // Assert
        Assert.NotNull(service);
        // Note: Constructor creates new DynamicRepository() when null is passed
    }

    [Fact]
    public void Constructor_ShouldUseDefaultCacheService_WhenCacheServiceIsNull()
    {
        // Arrange & Act
        var service = new SettingProfileService(
            _mockDynamicRepository.Object,
            _mockAppSettings.Object,
            null);

        // Assert
        Assert.NotNull(service);
        // Note: Constructor creates new CacheService() when null is passed
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenAppSettingsIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<NullReferenceException>(() => new SettingProfileService(
            _mockDynamicRepository.Object,
            null,
            _mockCacheService.Object));
    }

    #endregion

    #region Cache Configuration Tests

    [Fact]
    public async Task GetSettingProfiles_ShouldSetCorrectCacheExpiration()
    {
        // Arrange
        byte systemId = 1;
        string site = "TEST";
        int currency = 840;
        var mode = ProfileMode.AddNew;
        var serviceType = ServiceType.Credit;

        CacheItemOptions capturedOptions = null;

        _mockCacheService
            .Setup(x => x.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
                It.IsAny<CacheItemOptions>()))
            .Callback<string, Func<Task<IEnumerable<SettingProfile>>>, CacheItemOptions>(
                (key, factory, options) => capturedOptions = options)
            .ReturnsAsync(new List<SettingProfile>());

        // Act
        await _settingProfileService.GetSettingProfiles(systemId, site, currency, mode, serviceType);

        // Assert
        Assert.NotNull(capturedOptions);
        Assert.NotNull(capturedOptions.RelativeExpiration);
        Assert.Equal(new TimeSpan(1, 0, 0), capturedOptions.RelativeExpiration); // 1 hour
    }

    #endregion

    #region Edge Cases Tests

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)255)]
    public async Task GetSettingProfiles_ShouldHandleEdgeCaseSystemIds(byte systemId)
    {
        // Arrange
        string site = "TEST";
        int currency = 840;
        var mode = ProfileMode.AddNew;

        _mockCacheService
            .Setup(x => x.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
                It.IsAny<CacheItemOptions>()))
            .ReturnsAsync(new List<SettingProfile>());

        // Act & Assert
        var result = await _settingProfileService.GetSettingProfiles(systemId, site, currency, mode);
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("VERY_LONG_SITE_NAME_THAT_MIGHT_CAUSE_ISSUES")]
    public async Task GetSettingProfiles_ShouldHandleEdgeCaseSiteNames(string site)
    {
        // Arrange
        byte systemId = 1;
        int currency = 840;
        var mode = ProfileMode.AddNew;

        _mockCacheService
            .Setup(x => x.GetOrSetAsync(
                It.IsAny<string>(),
                It.IsAny<Func<Task<IEnumerable<SettingProfile>>>>(),
                It.IsAny<CacheItemOptions>()))
            .ReturnsAsync(new List<SettingProfile>());

        // Act & Assert
        var result = await _settingProfileService.GetSettingProfiles(systemId, site, currency, mode);
        Assert.NotNull(result);
    }

    #endregion
}
