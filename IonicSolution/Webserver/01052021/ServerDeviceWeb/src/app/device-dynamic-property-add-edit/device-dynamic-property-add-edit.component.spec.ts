import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceDynamicPropertyAddEditComponent } from './device-dynamic-property-add-edit.component';

describe('DeviceDynamicPropertyAddEditComponent', () => {
  let component: DeviceDynamicPropertyAddEditComponent;
  let fixture: ComponentFixture<DeviceDynamicPropertyAddEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeviceDynamicPropertyAddEditComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeviceDynamicPropertyAddEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
