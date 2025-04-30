import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApiLogTableComponent } from './api-log-table.component';

describe('ApiLogTableComponent', () => {
  let component: ApiLogTableComponent;
  let fixture: ComponentFixture<ApiLogTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ApiLogTableComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ApiLogTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
