import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { DeviceDynamicPropertyService } from '../device-dynamic-property.service';
import { DeviceDynamicProperty } from 'models/devicedynamicproperty';

@Component({
  selector: 'app-device-dynamic-properties',
  templateUrl: './device-dynamic-properties.component.html',
  styleUrls: ['./device-dynamic-properties.component.scss']
})
export class DeviceDynamicPropertiesComponent implements OnInit {
  deviceDynamicProperties$!: Observable<DeviceDynamicProperty[]>;

  constructor(private deviceDynamicPropertyService: DeviceDynamicPropertyService) {
  }

  ngOnInit() {
    this.loadDeviceDynamicProperties();
  }

  loadDeviceDynamicProperties() {
    this.deviceDynamicProperties$ = this.deviceDynamicPropertyService.getDeviceDynamicProperties();
    console.log("it is", this.deviceDynamicProperties$);

  }

  delete(propertyId: any) {
    const ans = confirm('Do you want to delete blog post with id: ' + propertyId);
    if (ans) {
      this.deviceDynamicPropertyService.deleteDeviceDynamicProperty(propertyId).subscribe((data) => {
        this.loadDeviceDynamicProperties();
      });
    }
  }
}