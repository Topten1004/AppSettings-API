import { AppSettingAuthentication } from './AppSettingAuthentication';
import { AppSettingFilter } from './AppSettingFilter';

export interface AppSettingReadRequest {
  Id?: number,
  AppSettingAuthentication: AppSettingAuthentication,
  FileName: string,
  UserObject: string,
  DefaultValue: string,
  AppSettingsFilter: AppSettingFilter
}
