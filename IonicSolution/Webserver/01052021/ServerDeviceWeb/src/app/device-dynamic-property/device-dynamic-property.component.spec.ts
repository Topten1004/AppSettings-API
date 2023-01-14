import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeviceDynamicPropertyComponent } from './device-dynamic-property.component';

describe('DeviceDynamicPropertyComponent', () => {
  let component: DeviceDynamicPropertyComponent;
  let fixture: ComponentFixture<DeviceDynamicPropertyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeviceDynamicPropertyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeviceDynamicPropertyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
