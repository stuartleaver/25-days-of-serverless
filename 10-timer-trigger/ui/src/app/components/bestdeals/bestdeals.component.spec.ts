import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BestdealsComponent } from './bestdeals.component';

describe('BestdealsComponent', () => {
  let component: BestdealsComponent;
  let fixture: ComponentFixture<BestdealsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BestdealsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BestdealsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
