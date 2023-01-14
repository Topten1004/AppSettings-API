import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { DeviceDetailService } from '../device-detail.service';
import { DeviceDetail } from 'models/devicedetail';
import { empty, interval } from 'rxjs';
import { catchError, mergeMap, startWith, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-device-detail-add-edit',
  templateUrl: './device-detail-add-edit.component.html',
  styleUrls: ['./device-detail-add-edit.component.scss']
})
export class DeviceDetailAddEditComponent implements OnInit {
  form: FormGroup;
  actionType: string;
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
  paramDeviceId: string;
  errorMessage: any;
  existingDeviceDetail!: DeviceDetail;
  formIsOnline: string;
  formdateClientDateTimeTicks: string;
  formdateServerDateTimeTicks: string
  constructor(private deviceDetailService: DeviceDetailService, private formBuilder: FormBuilder, private avRoute: ActivatedRoute, private router: Router) {
    const idParam = 'id';
    this.actionType = 'Add';
    this.paramDeviceId = '';
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
    if (this.avRoute.snapshot.params[idParam]) {
      this.paramDeviceId = this.avRoute.snapshot.params[idParam];
    }

    this.form = this.formBuilder.group(
      {
        strDeviceId: ['', [Validators.required]],
        strDeviceIds: ['', [Validators.required]],
        strVolume: ['', [Validators.required]],

        bytFaceDetectionQuality: ['', [Validators.required]],
        bytFaceDetectionMode: ['', [Validators.required]],
        ushortFaceDetectionFrameRateMaximum: ['', [Validators.required]],
        byteDetectionCount: ['', [Validators.required]],
        strPassword: ['', [Validators.required]],
        strShowDubugPanel: [false],
        boolUseSampling: [false],
        boolQrCodeActive: [false],
        boolIsOnline: [false],
        dateClientDateTimeTicks: [new Date()],
        dateServerDateTimeTicks: [new Date()]
      }
    )
  }

  ngOnInit() {

    if (this.paramDeviceId != '') {
      this.actionType = 'Edit';
      this.form.controls[this.formDeviceId].setValue(this.paramDeviceId),
        this.deviceDetailService.getDeviceDetail(this.paramDeviceId, null)
          .subscribe(data => (
            this.existingDeviceDetail = data,
            this.form.controls[this.formDeviceId].setValue(data.strDeviceId),
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
    }
    if (this.paramDeviceId != '' && false) {
      interval(6000)
        .pipe(
          startWith(0),
          mergeMap(() => this.deviceDetailService.getDeviceDetail(this.paramDeviceId, (this.existingDeviceDetail != null) ? this.existingDeviceDetail.dateServerDateTimeTicks : null).pipe(
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



  save() {
    if (!this.form.valid) {
      return;
    }

    if (this.actionType === 'Add') {
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
        dateServerDateTimeTicks: new Date()
      };

      this.deviceDetailService.saveDeviceDetail(deviceDetail)
        .subscribe((data) => {
          this.router.navigate(['/devicedetail', data.strDeviceId]);
        });
    }

    if (this.actionType === 'Edit') {
      let deviceDetail: DeviceDetail = {
        strDeviceIds: this.form.get(this.formDeviceIds)!.value,
        strVolume: this.form.get(this.formVolume)!.value,
        bytFaceDetectionQuality: this.form.get(this.formFaceDetectionQuality)!.value,
        bytFaceDetectionMode: this.form.get(this.formFaceDetectionMode)!.value,
        ushortFaceDetectionFrameRateMaximum: this.form.get(this.formFaceDetectionFrameRateMax)!.value,
        byteDetectionCount: this.form.get(this.formDetectionCount)!.value,
        strDeviceId: this.existingDeviceDetail.strDeviceId,
        strPassword: this.form.get(this.formPassword)!.value,
        strShowDubugPanel: this.form.get(this.formShowDubugPanel)!.value,
        boolUseSampling: this.form.get(this.formUseSampling)!.value,
        boolIsOnline: this.form.get(this.formIsOnline)!.value,
        boolQrCodeActive: this.form.get(this.formQRCodeActive)!.value,
        dateClientDateTimeTicks: new Date(),
        dateServerDateTimeTicks: this.existingDeviceDetail.dateServerDateTimeTicks,

      };
      this.deviceDetailService.updateDeviceDetail(deviceDetail.strDeviceId, deviceDetail)
        .subscribe((data) => {
          this.router.navigate([this.router.url]);
        });
    }
  }

  cancel() {
    this.router.navigate(['/']);
  }

  get strDeviceId() { return this.form.get(this.formDeviceId); }
  get strDeviceIds() { return this.form.get(this.formDeviceIds); }
  get bytFaceDetectionQuality() { return this.form.get(this.formFaceDetectionQuality); }
  get strVolume() { return this.form.get(this.formVolume); }
  get bytFaceDetectionMode() { return this.form.get(this.formFaceDetectionMode); }
  get ushortFaceDetectionFrameRateMaximum() { return this.form.get(this.formFaceDetectionFrameRateMax); }
  get byteDetectionCount() { return this.form.get(this.formDetectionCount); }

  get strPassword() { return this.form.get(this.formPassword); }
  get strShowDubugPanel() { return this.form.get(this.formShowDubugPanel); }
  get boolUseSampling() { return this.form.get(this.formUseSampling); }
  get boolQrCodeActive() { return this.form.get(this.formQRCodeActive); }
  get dateClientDateTimeTicks() { return this.form.get(this.formdateClientDateTimeTicks); }
  get dateServerDateTimeTicks() { return this.form.get(this.formdateServerDateTimeTicks); }

}


