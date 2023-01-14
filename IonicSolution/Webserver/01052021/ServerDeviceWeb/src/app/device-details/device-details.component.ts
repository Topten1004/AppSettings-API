import { Component, OnInit } from '@angular/core';
import { interval, Observable } from 'rxjs';
import { DeviceDetailService } from '../device-detail.service';
import { DeviceDetail } from 'models/devicedetail';

@Component({
  selector: 'app-device-details',
  templateUrl: './device-details.component.html',
  styleUrls: ['./device-details.component.scss']
})
export class DeviceDetailsComponent implements OnInit {
  deviceDetails$: Observable<DeviceDetail[]> | undefined;

  constructor(private deviceDetailService: DeviceDetailService) {
  }

  ngOnInit() {
    //this.loadDeviceDetails();
    interval(6000) .subscribe(data => this.loadDeviceDetails());
  }

  loadDeviceDetails() {
    this.deviceDetails$ = this.deviceDetailService.getDeviceDetails();
  }

  delete(deviceId: string) {
    const ans = confirm('Do you want to delete blog post with id: ' + deviceId);
    if (ans) {
      this.deviceDetailService.deleteDeviceDetail(deviceId).subscribe((data) => {
        this.loadDeviceDetails();
      });
    }
  }
}