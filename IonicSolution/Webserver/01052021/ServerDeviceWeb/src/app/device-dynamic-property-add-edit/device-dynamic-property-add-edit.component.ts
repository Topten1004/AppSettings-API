import { Component,  OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { DeviceDynamicPropertyService } from '../device-dynamic-property.service';
import { DeviceDynamicProperty } from 'models/devicedynamicproperty';



@Component({
  selector: 'app-device-dynamic-property-add-edit',
  templateUrl: './device-dynamic-property-add-edit.component.html',
  styleUrls: ['./device-dynamic-property-add-edit.component.scss']
})

export class DeviceDynamicPropertyAddEditComponent implements OnInit {
  form: FormGroup;
  actionType: string;
  displayTextProp: string;
  nameProp: string;
  isReadOnlyProp: string;
  placeHolderProp: string;
  typeProp: string;
  valueProp: string;
  propertyid!: number;
  errorMessage: any;
  existingDeviceDynamicProperty!: DeviceDynamicProperty;


  constructor(private deviceDynamicPropertyService: DeviceDynamicPropertyService, private formBuilder: FormBuilder, private avRoute: ActivatedRoute, private router: Router) {
    const idParam = 'id';
    this.actionType = 'Add';
    this.displayTextProp = 'displaytext';
    this.nameProp = 'name';
    this.isReadOnlyProp = 'isreadonly';
    this.placeHolderProp = 'placeholder';
    this.typeProp = 'type';
    this.valueProp = 'value';
    if (this.avRoute.snapshot.params[idParam]) {
      this.propertyid = this.avRoute.snapshot.params[idParam];
    }

    this.form = this.formBuilder.group(
      {
        propertyid: 0,
        displaytext: ['', [Validators.required]],
        name: ['', [Validators.required]],
        isreadonly: [false],
        placeholder: ['', [Validators.required]],
        type: ['', [Validators.required]],
        value: ['', [Validators.required]],
      }
    )
  }

  ngOnInit() {

    if (this.propertyid > 0) {
      this.actionType = 'Edit';
      this.deviceDynamicPropertyService.getDeviceDynamicProperty(this.propertyid)
        .subscribe(data => (
          this.existingDeviceDynamicProperty = data,
          this.form.controls[this.displayTextProp].setValue(data.displayText),
          this.form.controls[this.nameProp].setValue(data.name),
          this.form.controls[this.isReadOnlyProp].setValue(data.isReadOnly),
          this.form.controls[this.placeHolderProp].setValue(data.placeHolder),
          this.form.controls[this.typeProp].setValue(data.type),
          this.form.controls[this.valueProp].setValue(data.value)
        ));
    }
  }

  save() {
    if (!this.form.valid) {
      return;
    }

    if (this.actionType === 'Add') {
      let deviceDynamicProperty: DeviceDynamicProperty = {

        displayText: this.form.get(this.displayTextProp)!.value,
        name: this.form.get(this.nameProp)!.value,
        isReadOnly: this.form.get(this.isReadOnlyProp)!.value,
        type: this.form.get(this.typeProp)!.value,
        value: this.form.get(this.valueProp)!.value,
        placeHolder: this.form.get(this.placeHolderProp)!.value

      };

      this.deviceDynamicPropertyService.saveDeviceDynamicProperty(deviceDynamicProperty)
        .subscribe((data) => {
          this.router.navigate(['/devicedynamicproperty', data.propertyId]);
        });
    }

    if (this.actionType === 'Edit') {
      let deviceDynamicProperty: DeviceDynamicProperty = {
        propertyId: this.existingDeviceDynamicProperty.propertyId,
        displayText: this.form.get(this.displayTextProp)!.value,
        isReadOnly: this.form.get(this.isReadOnlyProp)!.value,
        value: this.form.get(this.valueProp)!.value,
        placeHolder: this.form.get(this.placeHolderProp)!.value,
        name: this.existingDeviceDynamicProperty.name,
        type: this.existingDeviceDynamicProperty.type

      };
      this.deviceDynamicPropertyService.updateDeviceDynamicProperty(deviceDynamicProperty.propertyId!, deviceDynamicProperty)
        .subscribe((data) => {
          this.router.navigate([this.router.url]);
        });
    }
  }

  cancel() {
    this.router.navigate(['/']);
  }
  get displaytext() { return this.form.get(this.displayTextProp); }
  get isreadonly() { return this.form.get(this.isReadOnlyProp); }
  get value() { return this.form.get(this.valueProp); }
  get placeholder() { return this.form.get(this.placeHolderProp); }
  get name() { return this.form.get(this.nameProp); }
  get type() { return this.form.get(this.typeProp); }

}