import { TestBed } from '@angular/core/testing';

import { ApiLogServiceService } from './api-log-service.service';

describe('ApiLogServiceService', () => {
  let service: ApiLogServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ApiLogServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
