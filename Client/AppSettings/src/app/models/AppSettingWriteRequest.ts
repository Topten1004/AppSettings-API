import { AppSettingAuthentication } from './AppSettingAuthentication';
import { AppSettingFilter } from './AppSettingFilter';

export interface AppSettingWriteRequest {
  AppSettingAuthentication: AppSettingAuthentication,
  FileName: string,
  Base64RawString: string,
  AppSettingsFilter: AppSettingFilter
}
