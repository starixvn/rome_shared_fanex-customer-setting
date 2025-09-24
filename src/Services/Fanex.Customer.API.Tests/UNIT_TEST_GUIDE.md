# COMPREHENSIVE UNIT TEST GUIDE

Đã cập nhật thành công:
✅ CreateCashMemberServiceTests.cs - 8 test cases
✅ CreateAgentFullModeServiceTests.cs - 6 test cases  
✅ CreateDirectMemberServiceTests.cs - 5 test cases

## 📋 PATTERN TO FOLLOW:

### 1. Test Structure:
```csharp
[Fact]
public async Task Create_ShouldReturnFailure_WhenValidationFails()
[Fact] 
public async Task Create_ShouldReturnFailure_WhenRegistrationFails()
[Fact]
public async Task Create_ShouldReturnFailure_WhenCreate[Entity]Fails()
[Fact]
public async Task Create_ShouldReturnSuccess_WhenAllStepsSucceed()
[Fact]
public async Task Create_ShouldHandle[Entity]SpecificLogic()
```

### 2. Setup Methods Pattern:
- `SetupValidationSuccess()` & `SetupValidationFailure()`
- `SetupRegistrationSuccess()` & `SetupRegistrationFailure()`  
- `SetupCreate[Entity]Success()` & `SetupCreate[Entity]Failure()`
- `VerifyRollbackCalled()` & `Verify[Entity]CreationCalled()`

### 3. Files Still Need Updates:

**FullMode:**
- CreateMemberFullModeServiceTests.cs
- CreateSuperFullModeServiceTests.cs

**LiteMode:**
- CreateAgentLiteModeServiceTests.cs
- CreateMasterLiteModeServiceTests.cs  
- CreateMemberLiteModeServiceTests.cs

**Base:**
- CreateCustomerBaseServiceTests.cs

### 4. Entity Mapping:

| Service | Entity | Command | Validation | CustomerLevel |
|---------|--------|---------|------------|---------------|
| CreateMemberFullMode | Member | CreateMemberCommand | ValidateMemberSetting | CustomerLevel.Member |
| CreateSuperFullMode | Super | CreateSuperCommand | ValidateSuperSetting | CustomerLevel.Super |
| CreateAgentLiteMode | Agent | CreateAgentCommand | ValidateAgentSetting | CustomerLevel.Agent |
| CreateMasterLiteMode | Master | CreateMasterCommand | ValidateMasterSetting | CustomerLevel.Master |
| CreateMemberLiteMode | Member | CreateMemberCommand | ValidateMemberSetting | CustomerLevel.Member |

### 5. Key Dependencies to Mock:
- `Mock<IMessageService>`
- `Mock<ICustomerSettingHelper>`  
- `Mock<IDynamicRepository>`
- `AppSettings` (concrete class)

### 6. Test Request Setup:
```csharp
private CreateRequest CreateTestRequest()
{
    return new CreateRequest
    {
        Context = new CreateRequestContext { /* standard context */ },
        Settings = new CustomerSetting
        {
            GeneralInfo = new GeneralInfo 
            { 
                /* entity-specific properties */
                RoleId = CustomerLevel.[EntityLevel],
                UplineRoleId = CustomerLevel.[UplineLevel]
            },
            ProductGroups = new List<ProductGroup>()
        }
    };
}
```

## 🎯 BENEFITS ACHIEVED:
- **Comprehensive coverage** thay vì 1 basic test
- **Proper mocking** của dependencies
- **Edge case testing** (validation, registration, creation failures)
- **Rollback verification**
- **Entity-specific logic testing**

## 💡 NEXT STEPS:
Apply the same pattern to remaining files using the examples from the 3 completed files as reference.