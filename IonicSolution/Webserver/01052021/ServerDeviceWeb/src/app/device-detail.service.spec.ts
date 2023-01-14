import { TestBed } from '@angular/core/testing';

import { DeviceDetailService } from './device-detail.service';

describe('DeviceDetailService', () => {
  let service: DeviceDetailService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeviceDetailService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
