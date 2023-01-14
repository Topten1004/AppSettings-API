import { Injectable } from '@angular/core';
import {
  Plugins, CameraResultType, Capacitor, FilesystemDirectory,
  CameraPhoto, CameraSource
} from '@capacitor/core';
import { HttpClient } from '@angular/common/http';

import { ToastController } from '@ionic/angular';
import { Platform } from '@ionic/angular';
import { Observable } from 'rxjs';
import { AppSettingResponse } from '../models/AppSettingResponse';
import { AppSettingResponseDetails } from '../models/AppSettingResponseDetails';
import { AppSettingWriteRequest } from '../models/AppSettingWriteRequest';
import { AppSettingFilter } from '../models/AppSettingFilter';
import { AppSettingAuthentication } from '../models/AppSettingAuthentication';
import { AppSettingReadRequest } from '../models/AppSettingReadRequest';

const { Camera, Filesystem, Storage } = Plugins;


@Injectable({
  providedIn: 'root'
})

export class PhotoService {
  private apiUrl: string = "http://t78.ch/appsettings";
  //private apiUrl: string = "http://localhost:5000";
  private apiPage: string = "/api/Db"


  private platform: Platform;
  constructor(public toastCtrl: ToastController, private http: HttpClient, platform: Platform) {
    this.http = http;
    this.platform = platform;

  }

  async openToast() {
    const toast = await this.toastCtrl.create({
      message: 'Image Uploaded Succesfully',
      duration: 4000
    });
    toast.present();
  }
  public photos: Photo[] = [];
  public appSettingDataObject: any

  public async addNewToGallery() {

    // Take a photo
    const capturedPhoto = await Camera.getPhoto({
      resultType: CameraResultType.Uri, // file-based data; provides best performance
      source: CameraSource.Camera, // automatically take a new photo with the camera
      quality: 100 // highest quality (0 to 100)
    });

    // Save the picture and add it to photo collection
    const savedImageFile = await this.savePicture(capturedPhoto);
    this.photos.unshift(savedImageFile);
  }

  private async savePicture(cameraPhoto: CameraPhoto) : Promise<Photo>{

    // Convert photo to base64 format, required by Filesystem API to save
    const base64Data = await this.readAsBase64(cameraPhoto);

    // Write the file to the data directory
    const fileName = new Date().getTime() + '.jpeg';
    const savedFile = await Filesystem.writeFile({
      path: fileName,
      data: base64Data,
      directory: FilesystemDirectory.Data
    });

    var appSettingAuthentication = this.GetAuthentification();
    var appSettingFilter = this.GetTestFilter();

    var appSettingWriteRequest: AppSettingWriteRequest = {
      AppSettingAuthentication: appSettingAuthentication,
      Base64RawString: base64Data,
      FileName: fileName,
      AppSettingsFilter: appSettingFilter
    }

    //Have Hard-coded the values so later can be set by user-input.
    this.WriteAppSettings(appSettingWriteRequest);
    // Use webPath to display the new image instead of base64 since it's
    // already loaded into memory
    return {
      filepath: fileName,
      webviewPath: cameraPhoto.webPath
    };

  }


  // Main function to write settings
  // public async WriteAppSettings(key1: string, key2: string, key3: string, key4: string, appSettingsDataObject: Image) {
  //   const appSettingObject = {
  //     Image: appSettingsDataObject.imageBase64,
  //     Key1: key1,
  //     Key2: key2,
  //     Key3: key3,
  //     Key4: key4
  //   }
  //   this.http.post("http://localhost:61448/api/appSettings", appSettingObject)
  //     .pipe()
  //     .subscribe(res => {
  //       this.savedPhotos = res; // why response is a collection of images here when writing data?

  //       this.openToast();

  //     });
  // }

  // private Base64ToLength(appSettingSenderObject: AppSettingDataValue): number {
  //   if (appSettingSenderObject == null) return 0;
  //   if (appSettingSenderObject.Base64RawString == null) return 0;
  //   if ((appSettingSenderObject.FileName + '').length == 0) return 0;
  //   var length = appSettingSenderObject.Base64RawString.length;
  //   var rawLength = ((4 * length / 3) + 3) & ~3;
  //   return rawLength;
  // }

  // Main function to write settings
  public async WriteAppSettings(appSettingWriteRequest: AppSettingWriteRequest) {
    // var dateNow: Date = new Date();
    // var rawLength : number = this.Base64ToLength(appSettingSenderObject);
    // var appSettingDataDescriptor: AppSettingDataDescriptor = {
    //   FileName: appSettingSenderObject.FileName,
    //   Length: rawLength,
    //   DateCreated: dateNow,
    //   DateLastWrite: dateNow,
    //   DateLastRead: dateNow,
    //   MediaType: "",
    //   AppSettingDataObjectId: null
    // }

    // const appSettingDataObject: AppSettingDataObject = {
    //   AppSettingDataDescriptor: appSettingDataDescriptor,
    //   Key1: key1,
    //   Key2: key2,
    //   Key3: key3,
    //   Key4: key4,
    //   LastError: "",
    //   Base64RawString: ""
    // }

    this.http.post(this.apiUrl + this.apiPage + "/Write", appSettingWriteRequest)
      .pipe()
      .subscribe(res => {
        this.appSettingDataObject = res;

        this.openToast();

      });
  }

  private async readAsBase64(cameraPhoto: CameraPhoto) {


    // Fetch the photo, read as a blob, then convert to base64 format
    const response = await fetch(cameraPhoto.webPath!);
    const blob = await response.blob();

    return await this.convertBlobToBase64(blob) as string;
  }

  convertBlobToBase64 = (blob: Blob) => new Promise((resolve, reject) => {
    const reader = new FileReader;
    reader.onerror = reject;
    reader.onload = () => {
      resolve(reader.result);
    };
    reader.readAsDataURL(blob);
  });


  // public async LoadAppSettings_(key1: string, key2: string, key3: string, key4: string) {
  //   // Retrieve cached photo array data

  //   let data: Observable<any>;
  //   let appSettingObject = {
  //     Key1: key1,
  //     Key2: key2,
  //     Key3: key3,
  //     Key4: key4
  //   }
  //   this.http.post("http://localhost:61448/api/Read", appSettingObject)
  //     .pipe()
  //     .subscribe(res: AppSettingDataObject => {
  //       return res;

  //       //this.openToast();

  //     });
  //   // data.subscribe(res => {

  //   //   this.savedPhotos = res;
  //   // },
  //   //   err => {
  //   //     // Set the error information to display in the template
  //   //     console.log(`An error occurred, the data could not be retrieved: Status: ${err.status}, Message: ${err.statusText}`);
  //   //   });

  // }


  // Main function to write settings
  public async LoadAppSettings(key1: string, key2: string, key3: string, key4: string) {
    var appSettingAuthentication = this.GetAuthentification()
    var userObject = new Date().getTime().toString();
    var appSettingFilter = this.GetTestFilter();
    var appSettingReadRequest: AppSettingReadRequest = {
      AppSettingsFilter: appSettingFilter,
      AppSettingAuthentication: appSettingAuthentication,
      FileName: "",
      UserObject: userObject,
      DefaultValue: ""

    };

    // "http://localhost:61448/
    this.http.post(this.apiUrl + this.apiPage + "/Read", appSettingReadRequest)
      .pipe()
      .subscribe(res => {
        this.appSettingDataObject = res;

        this.openToast();

      });
  }

  private GetAuthentification(): AppSettingAuthentication {
    var appSettingAuthentication: AppSettingAuthentication = {
      Version: "1.0.0.0",
      ApiKey: "0",
      Secret: "X",
      Hash: "100",
      Message: ''
    }
    return appSettingAuthentication;
  }

  public GetTestFilter(): AppSettingFilter {
    var appSettingFilter: AppSettingFilter = {
      ApplicationName: "ApplicationName",
      RootKey: "RootKey",
      RegionKey: "RegionKey",
      PropertyName: "PropertyName"
    }
    return appSettingFilter;
  }
}

export interface Photo {
  filepath: string;
  webviewPath: string;
}


export interface Image {
  imageBase64: string
}
