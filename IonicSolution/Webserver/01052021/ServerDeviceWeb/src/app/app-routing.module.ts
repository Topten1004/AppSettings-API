import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeviceDynamicPropertiesComponent } from './device-dynamic-properties/device-dynamic-properties.component';
import { DeviceDynamicPropertyComponent } from './device-dynamic-property/device-dynamic-property.component';
import { DeviceDynamicPropertyAddEditComponent } from './device-dynamic-property-add-edit/device-dynamic-property-add-edit.component';

import { DeviceDetailAddEditComponent } from './device-detail-add-edit/device-detail-add-edit.component';
import { DeviceDetailsComponent } from './device-details/device-details.component';

const routes: Routes = [
  //{ path: '', component: DeviceDynamicPropertiesComponent, pathMatch: 'full' },
  { path: 'devicedynamicproperty/:id', component: DeviceDynamicPropertyComponent },
  { path: 'add', component: DeviceDynamicPropertyAddEditComponent },
  { path: 'devicedynamicproperty/edit/:id', component: DeviceDynamicPropertyAddEditComponent },
  { path: '', component: DeviceDetailsComponent, pathMatch: 'full' },
  
  { path: 'devicedetail/add', component: DeviceDetailAddEditComponent },
  { path: 'devicedetail/edit/:id', component: DeviceDetailAddEditComponent },
  { path: '**', redirectTo: '/' }
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
