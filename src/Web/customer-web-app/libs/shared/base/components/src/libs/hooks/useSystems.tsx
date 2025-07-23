import { SystemIds } from "@customer-web-app/shared-constants";

export const useSystems = (systemId: number) => {
    // Check if the systemId is a valid SystemIds enum value
    const isValidSystemId = (id: number): boolean => {
      return Object.values(SystemIds).includes(id);
    };
  
    // Get system name by ID
    const getSystemName = (id: number): string => {
      const systemName = Object.keys(SystemIds).find(
        key => SystemIds[key as keyof typeof SystemIds] === id
      );
      return systemName || 'Unknown';
    };
  
    // Individual system checks
    const isNap = systemId === SystemIds.NAP;
    const isAlpha = systemId === SystemIds.Alpha;
    const isInternalAlpha = systemId === SystemIds.InternalAlpha;
    const isExternalNap = systemId === SystemIds.ExternalNAP;
    const isExternalNapBetSetting = systemId === SystemIds.ExternalNAPBetSetting;
    const isY789Alpha = systemId === SystemIds.y789Alpha;
    const isY789InternalAlpha = systemId === SystemIds.y789InternalAlpha;
    const isExternalNapSpecialSite = systemId === SystemIds.ExternalNapSpecialSite;
    const isExternalNapBetSettingSpecialSite = systemId === SystemIds.ExternalNAPBetSettingSpecialSite;
  
    // Group checks
    const isAlphaSystem = [
      SystemIds.Alpha,
      SystemIds.y789Alpha,
      SystemIds.y789InternalAlpha,
      SystemIds.InternalAlpha,
    ].includes(systemId);
  
    const isExternalNapSystem = [
      SystemIds.ExternalNAP,
      SystemIds.ExternalNAPBetSetting,
      SystemIds.ExternalNapSpecialSite,
      SystemIds.ExternalNAPBetSettingSpecialSite,
    ].includes(systemId);
  
    const isSpecialSystem = [
      SystemIds.y789Alpha,
      SystemIds.y789InternalAlpha,
      SystemIds.ExternalNapSpecialSite,
      SystemIds.ExternalNAPBetSettingSpecialSite,
    ].includes(systemId);
  
    const isEditBetSettingOnly = [
      SystemIds.ExternalNAPBetSetting,
      SystemIds.ExternalNAPBetSettingSpecialSite,
    ].includes(systemId);
  
    // Utility functions
    const isExternalNapOrSpecial = systemId === SystemIds.ExternalNAP || systemId === SystemIds.ExternalNapSpecialSite;
  
    return {
      // System ID validation
      isValidSystemId: isValidSystemId(systemId),
      systemName: getSystemName(systemId),
      
      // Individual system checks
      isNap,
      isAlpha,
      isInternalAlpha,
      isExternalNap,
      isExternalNapBetSetting,
      isY789Alpha,
      isY789InternalAlpha,
      isExternalNapSpecialSite,
      isExternalNapBetSettingSpecialSite,
      
      // Group checks
      isAlphaSystem,
      isExternalNapSystem,
      isSpecialSystem,
      isEditBetSettingOnly,
      isExternalNapOrSpecial,
      
      // Raw system ID
      systemId,
    };
  };
  