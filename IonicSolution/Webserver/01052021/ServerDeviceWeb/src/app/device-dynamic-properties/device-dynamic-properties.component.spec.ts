import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceDynamicPropertiesComponent } from './device-dynamic-properties.component';

describe('DeviceDynamicPropertiesComponent', () => {
  let component: DeviceDynamicPropertiesComponent;
  let fixture: ComponentFixture<DeviceDynamicPropertiesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeviceDynamicPropertiesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeviceDynamicPropertiesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
