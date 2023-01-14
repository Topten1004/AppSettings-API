import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DeviceDynamicPropertiesComponent } from './device-dynamic-properties/device-dynamic-properties.component';
import { DeviceDynamicPropertyComponent } from './device-dynamic-property/device-dynamic-property.component';

import { DeviceDynamicPropertyAddEditComponent } from './device-dynamic-property-add-edit/device-dynamic-property-add-edit.component';
import { DeviceDynamicPropertyService } from './device-dynamic-property.service';
import { FormsModule } from '@angular/forms';
import { DeviceDetailsComponent } from './device-details/device-details.component';
import { DeviceDetailComponent } from './device-detail/device-detail.component';
import { DeviceDetailAddEditComponent } from './device-detail-add-edit/device-detail-add-edit.component';
import { DeviceDetailService } from './device-detail.service';
@NgModule({
  declarations: [
    AppComponent,
    DeviceDynamicPropertiesComponent,
    DeviceDynamicPropertyComponent,
    DeviceDynamicPropertyAddEditComponent,
    DeviceDetailsComponent,
    DeviceDetailComponent,
    DeviceDetailAddEditComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule
  ],
  providers: [DeviceDynamicPropertyService],
  bootstrap: [AppComponent]
})
export class AppModule { }
