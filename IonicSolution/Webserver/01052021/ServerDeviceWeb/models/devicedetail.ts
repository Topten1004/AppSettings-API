import { Byte } from "@angular/compiler/src/util"

export class DeviceDetail {

    strDeviceId: string;
    strDeviceIds: string;
    strPassword: string;
    strVolume: number;
    strShowDubugPanel: boolean;
    boolUseSampling: boolean;
    boolIsOnline: boolean;
    bytFaceDetectionQuality: number;
    bytFaceDetectionMode: number;
    ushortFaceDetectionFrameRateMaximum: number;
    boolQrCodeActive: boolean;
    byteDetectionCount: number;
    dateClientDateTimeTicks: Date
    dateServerDateTimeTicks: Date
    constructor() {
        this.strDeviceId = '';
        this.strDeviceIds = '';
        this.strPassword = '';
        this.strVolume = 0;
        this.strShowDubugPanel = false;
        this.boolUseSampling = false;
        this.boolIsOnline = false;
        this.bytFaceDetectionQuality = 0;
        this.bytFaceDetectionMode = 0;
        this.ushortFaceDetectionFrameRateMaximum = 0;
        this.boolQrCodeActive = false;
        this.byteDetectionCount = 0;
        this.dateClientDateTimeTicks = new Date();
        this.dateServerDateTimeTicks = new Date();
    }
}