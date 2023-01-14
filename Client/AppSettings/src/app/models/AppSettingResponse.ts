import { AppSettingAuthentication } from './AppSettingAuthentication';
import { AppSettingResponseDetails } from './AppSettingResponseDetails';

export interface AppSettingResponse {
    Id?: number,
    AppSettingAuthentication: AppSettingAuthentication,
    ApplicationName: string,
    RootKey: string,
    RegionKey: string,
    PropertyName: string,
    Base64RawString: string,
    AppSettingResponseDetails: AppSettingResponseDetails,
    LastError: string
  }
  