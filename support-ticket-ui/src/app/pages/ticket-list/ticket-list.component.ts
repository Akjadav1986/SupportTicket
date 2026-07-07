import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject, finalize, takeUntil, timeout } from 'rxjs';
import { TicketService } from '../../services/ticket.service';
import { TicketResponse } from '../../models/ticket.model';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ticket-list.component.html'
})
export class TicketListComponent implements OnInit, OnDestroy {
  tickets: TicketResponse[] = [];
  isLoading = false;
  errorMessage = '';

  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly ticketService: TicketService,
    private readonly changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadTickets();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadTickets(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.ticketService.getTickets()
      .pipe(
        timeout(15000),
        takeUntil(this.destroy$),
        finalize(() => {
          this.isLoading = false;
          this.refreshView();
        })
      )
      .subscribe({
        next: (tickets) => {
          this.tickets = tickets ?? [];
          this.refreshView();
        },
        error: (error) => {
          console.error('Ticket loading failed:', error);
          this.errorMessage = 'Unable to load tickets. Please check API is running, API URL, SQL Server connection and CORS settings.';
          this.refreshView();
        }
      });
  }

  reclassify(ticket: TicketResponse): void {
    this.errorMessage = '';

    this.ticketService.reclassify(ticket.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (updatedTicket) => {
          this.tickets = this.tickets.map(x => x.id === updatedTicket.id ? updatedTicket : x);
          this.refreshView();
        },
        error: (error) => {
          console.error('Ticket reclassification failed:', error);
          this.errorMessage = 'Unable to reclassify ticket.';
          this.refreshView();
        }
      });
  }

  private refreshView(): void {
    try {
      this.changeDetectorRef.detectChanges();
    } catch {
      // Component can be destroyed while an HTTP call is completing.
    }
  }
}