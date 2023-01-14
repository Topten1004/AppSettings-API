import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { DeviceDetail } from 'models/devicedetail';
import { DeviceDetailService } from '../device-detail.service';
import { UniqueDeviceID } from '@ionic-native/unique-device-id/ngx';
import { Platform } from '@ionic/angular';
import { empty, interval, of } from 'rxjs';
import { catchError, filter, mergeMap, startWith, switchMap } from 'rxjs/operators';
@Component({

  selector: 'app-home',
  templateUrl: './home.page.html',
  styleUrls: ['./home.page.scss'],
})
export class HomePage implements OnInit {

  public form: FormGroup;

  formDeviceId: string;
  formDeviceIds: string;
  formShowDubugPanel: string;
  formPassword: string;
  formVolume: string;
  formUseSampling: string;
  formFaceDetectionQuality: string;
  formFaceDetectionMode: string;
  formFaceDetectionFrameRateMax: string;
  formQRCodeActive: string;
  formDetectionCount: string;
  formIsOnline: string;
  formdateClientDateTimeTicks: string;
  formdateServerDateTimeTicks: string
  myId = null;
  public submitAttempt: boolean = false;
  existingDeviceDetail!: DeviceDetail;
  public uniqueId: any;
  constructor(public formBuilder: FormBuilder, private deviceDetailService: DeviceDetailService, private router: Router, private uniqueDeviceID: UniqueDeviceID, public platform: Platform, private activatedRoute: ActivatedRoute) {
    if (this.platform.is('cordova')) {
      // You are on a device, cordova plugins are accessible
      this.uniqueId = this.uniqueDeviceID.get()
        .then((uuid: any) => console.log(uuid))
        .catch((error: any) => console.log(error));
    } else {
      // Cordova not accessible, add mock data if necessary
      this.uniqueId = this.activatedRoute.snapshot.paramMap.get('id');

    }

    if (this.uniqueId == null) {
      this.uniqueId = "1";
    }


    this.formDeviceId = 'strDeviceId';
    this.formDeviceIds = 'strDeviceIds';
    this.formShowDubugPanel = 'strShowDubugPanel';
    this.formPassword = 'strPassword';
    this.formVolume = 'strVolume';
    this.formUseSampling = 'boolUseSampling';
    this.formFaceDetectionQuality = 'bytFaceDetectionQuality';
    this.formFaceDetectionMode = 'bytFaceDetectionMode';
    this.formFaceDetectionFrameRateMax = 'ushortFaceDetectionFrameRateMaximum';
    this.formQRCodeActive = 'boolQrCodeActive';
    this.formDetectionCount = 'byteDetectionCount';
    this.formIsOnline = 'boolIsOnline';
    this.formdateClientDateTimeTicks = 'dateClientDateTimeTicks';
    this.formdateServerDateTimeTicks = 'dateServerDateTimeTicks';
    this.form = formBuilder.group({
      strDeviceId: [this.uniqueId, Validators.compose([Validators.required]), null],
      strDeviceIds: ['', Validators.compose([Validators.required])],
      strVolume: ['', Validators.compose([Validators.required])],
      bytFaceDetectionQuality: ['', Validators.compose([Validators.required])],
      bytFaceDetectionMode: ['', Validators.compose([Validators.required])],
      ushortFaceDetectionFrameRateMaximum: ['', Validators.compose([Validators.required])],
      byteDetectionCount: ['', Validators.compose([Validators.required])],
      strPassword: ['', Validators.compose([Validators.required])],
      strShowDubugPanel: [false, Validators.compose([Validators.required])],
      boolUseSampling: [false, Validators.compose([Validators.required])],
      boolQrCodeActive: [false, Validators.compose([Validators.required])],
      boolIsOnline: [false, Validators.compose([Validators.required])],
      dateClientDateTimeTicks: [new Date()],
      dateServerDateTimeTicks: [new Date()]
    });


  }

  save() {

    if (!this.form.valid) {
      this.submitAttempt = true;
      return;
    } else {
      this.submitAttempt = false;
      let deviceDetail: DeviceDetail = {
        strDeviceIds: this.form.get(this.formDeviceIds)!.value,
        strVolume: this.form.get(this.formVolume)!.value,
        bytFaceDetectionQuality: this.form.get(this.formFaceDetectionQuality)!.value,
        bytFaceDetectionMode: this.form.get(this.formFaceDetectionMode)!.value,
        ushortFaceDetectionFrameRateMaximum: this.form.get(this.formFaceDetectionFrameRateMax)!.value,
        byteDetectionCount: this.form.get(this.formDetectionCount)!.value,
        strDeviceId: this.form.get(this.formDeviceId)!.value,
        strPassword: this.form.get(this.formPassword)!.value,
        strShowDubugPanel: this.form.get(this.formShowDubugPanel)!.value,
        boolUseSampling: this.form.get(this.formUseSampling)!.value,
        boolIsOnline: this.form.get(this.formIsOnline)!.value,
        boolQrCodeActive: this.form.get(this.formQRCodeActive)!.value,
        dateClientDateTimeTicks: new Date(),
        dateServerDateTimeTicks: new Date(),

      };
      this.deviceDetailService.updateDeviceDetail(deviceDetail.strDeviceId, deviceDetail)
        .subscribe((data) => {
          //this.router.navigate([this.router.url]);
          console.log([this.router.url]);
        });

    }

  }





  ngOnInit() {

    this.deviceDetailService.getDeviceDetail(this.uniqueId, null)
      .subscribe(data => (
        this.existingDeviceDetail = data,
        //this.form.controls[this.formDeviceId].setValue(data.strDeviceId),
        this.form.controls[this.formDeviceIds].setValue(data.strDeviceIds),
        this.form.controls[this.formVolume].setValue(data.strVolume),
        this.form.controls[this.formFaceDetectionQuality].setValue(data.bytFaceDetectionQuality),
        this.form.controls[this.formFaceDetectionMode].setValue(data.bytFaceDetectionMode),
        this.form.controls[this.formFaceDetectionFrameRateMax].setValue(data.ushortFaceDetectionFrameRateMaximum),
        this.form.controls[this.formDetectionCount].setValue(data.byteDetectionCount),
        this.form.controls[this.formShowDubugPanel].setValue(data.strShowDubugPanel),
        this.form.controls[this.formQRCodeActive].setValue(data.boolQrCodeActive),
        this.form.controls[this.formUseSampling].setValue(data.boolUseSampling),
        this.form.controls[this.formPassword].setValue(data.strPassword),
        this.form.controls[this.formIsOnline].setValue(data.boolIsOnline),
        this.form.controls[this.formdateServerDateTimeTicks].setValue(data.dateServerDateTimeTicks),
        this.form.controls[this.formdateClientDateTimeTicks].setValue(data.dateClientDateTimeTicks)
      ));




    if (false) {
      interval(10000)
        .pipe(
          startWith(0),
          mergeMap(() => this.deviceDetailService.getDeviceDetail(this.uniqueId, (this.existingDeviceDetail != null) ? this.existingDeviceDetail.dateServerDateTimeTicks : null).pipe(
            catchError((error) => {
              console.log(error);
              return empty(); // or return of(error) and do sth about it in the subscribe body
            }),

          ))
        )


        .subscribe(data => {
          if (data) {
            this.existingDeviceDetail = data,
              //this.form.controls[this.formDeviceId].setValue(data.strDeviceId),
              this.form.controls[this.formDeviceIds].setValue(data.strDeviceIds),
              this.form.controls[this.formVolume].setValue(data.strVolume),
              this.form.controls[this.formFaceDetectionQuality].setValue(data.bytFaceDetectionQuality),
              this.form.controls[this.formFaceDetectionMode].setValue(data.bytFaceDetectionMode),
              this.form.controls[this.formFaceDetectionFrameRateMax].setValue(data.ushortFaceDetectionFrameRateMaximum),
              this.form.controls[this.formDetectionCount].setValue(data.byteDetectionCount),
              this.form.controls[this.formShowDubugPanel].setValue(data.strShowDubugPanel),
              this.form.controls[this.formQRCodeActive].setValue(data.boolQrCodeActive),
              this.form.controls[this.formUseSampling].setValue(data.boolUseSampling),
              this.form.controls[this.formPassword].setValue(data.strPassword),
              this.form.controls[this.formIsOnline].setValue(data.boolIsOnline),
              this.form.controls[this.formdateServerDateTimeTicks].setValue(data.dateServerDateTimeTicks),
              this.form.controls[this.formdateClientDateTimeTicks].setValue(data.dateClientDateTimeTicks)
          }
        });
    }
  }

}
function AppError(arg0: { error: any; }): any {
  console.log(arg0);
}

