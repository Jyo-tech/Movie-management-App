import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BehaviorSubject } from 'rxjs';
import { CommonModule } from '@angular/common';

import { NotificationComponent } from './notification';
import { Notification, NotificationMessage } from '../../core/notification';

describe('NotificationComponent', () => {
  let component: NotificationComponent;
  let fixture: ComponentFixture<NotificationComponent>;
  let notificationSubject: BehaviorSubject<NotificationMessage | null>;
  let notificationService: any;

  beforeEach(async () => {
    notificationSubject = new BehaviorSubject<NotificationMessage | null>(null);
    notificationService = {
      notification$: notificationSubject.asObservable()
    };

    await TestBed.configureTestingModule({
      imports: [CommonModule],
      declarations: [NotificationComponent],
      providers: [{ provide: Notification, useValue: notificationService }]
    }).compileComponents();

    fixture = TestBed.createComponent(NotificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show toast when notification is emitted', () => {
    notificationSubject.next({ type: 'success', message: 'Hello' });
    fixture.detectChanges();

    const toast = fixture.nativeElement.querySelector('.toast-container');
    expect(toast).toBeTruthy();
    expect(toast.textContent).toContain('Hello');
  });

  it('should hide toast when notification becomes null', () => {
    notificationSubject.next({ type: 'success', message: 'Hello' });
    fixture.detectChanges();

    notificationSubject.next(null);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.toast-container')).toBeNull();
  });

  it('should call markForCheck when notification updates', () => {
    const cdr = fixture.debugElement.injector.get<any>('ChangeDetectorRef');
    const markSpy = jest.spyOn(cdr, 'markForCheck');

    notificationSubject.next({ type: 'error', message: 'Oops' });
    expect(markSpy).toHaveBeenCalled();
  });
});
