import { Injectable } from '@angular/core';
import { Plugins, CameraResultType, Capacitor, FilesystemDirectory, 
  CameraPhoto, CameraSource } from '@capacitor/core';
  import { HttpClient } from '@angular/common/http';
  
  import { ToastController } from '@ionic/angular';  
  import { Platform } from '@ionic/angular';
import { Observable } from 'rxjs';

const { Camera, Filesystem, Storage } = Plugins;


@Injectable({
  providedIn: 'root'
})

export class PhotoService {
  private platform: Platform;
  constructor( public toastCtrl: ToastController , private http : HttpClient,platform: Platform) { 
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
  public savedPhotos : any

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

  private  async  savePicture(cameraPhoto : CameraPhoto){

     // Convert photo to base64 format, required by Filesystem API to save
    const base64Data = await this.readAsBase64(cameraPhoto);

     // Write the file to the data directory
      const fileName = new Date().getTime() + '.jpeg';
      const savedFile = await Filesystem.writeFile({
        path: fileName,
        data: base64Data,
        directory: FilesystemDirectory.Data
      });

    
      const obj : Image = {
         imageBase64 : base64Data
      }

      //Have Hard-coded the values so later can be set by user-input.
      this.WriteAppSettings("KeyA","KeyB","KeyC","KeyD" , obj);
      // Use webPath to display the new image instead of base64 since it's
      // already loaded into memory
      return {
        filepath: fileName,
        webviewPath: cameraPhoto.webPath
      };

  }

  private async WriteAppSettings(key1:string , key2: string , key3:string , key4:string , appSettingsDataObject : Image){


    const obj = {
      Image : appSettingsDataObject.imageBase64,
      Key1  : key1,
      Key2 : key2,
      Key3 : key3,
      Key4 : key4
    }
    this.http.post("http://localhost:61448/api/appSettings", obj)
    .pipe(
        
    )
    .subscribe(res => {
      this.savedPhotos = res;
    
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

  public async loadSaved() {
    // Retrieve cached photo array data
    
    let data : Observable<any>;
    data = this.http.get("http://localhost:61448/api/AppSettings");
    data.subscribe(res => {

          this.savedPhotos = res; 
        },
        err => {
          // Set the error information to display in the template
          console.log(`An error occurred, the data could not be retrieved: Status: ${err.status}, Message: ${err.statusText}`);
        });
   
  }


}


export interface Photo {
  filepath: string;
  webviewPath: string;
}


export interface Image{
  imageBase64 : string
}
