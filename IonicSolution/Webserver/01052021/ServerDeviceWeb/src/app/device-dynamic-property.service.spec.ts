import { TestBed } from '@angular/core/testing';

import { DeviceDynamicPropertyService } from './device-dynamic-property.service';

describe('DeviceDynamicPropertyService', () => {
  let service: DeviceDynamicPropertyService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeviceDynamicPropertyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
