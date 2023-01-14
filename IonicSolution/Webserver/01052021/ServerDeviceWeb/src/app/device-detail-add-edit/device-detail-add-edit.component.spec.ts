import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceDetailAddEditComponent } from './device-detail-add-edit.component';

describe('DeviceDetailAddEditComponent', () => {
  let component: DeviceDetailAddEditComponent;
  let fixture: ComponentFixture<DeviceDetailAddEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeviceDetailAddEditComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeviceDetailAddEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
