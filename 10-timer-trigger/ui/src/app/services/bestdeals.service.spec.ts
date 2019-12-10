import { TestBed } from '@angular/core/testing';

import { BestdealsService } from './bestdeals.service';

describe('BestdealsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: BestdealsService = TestBed.get(BestdealsService);
    expect(service).toBeTruthy();
  });
});
