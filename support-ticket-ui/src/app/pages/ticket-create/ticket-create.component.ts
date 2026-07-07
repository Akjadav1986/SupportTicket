import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TicketService } from '../../services/ticket.service';
import { CreateTicketRequest, TicketResponse } from '../../models/ticket.model';

@Component({
  selector: 'app-ticket-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ticket-create.component.html'
})
export class TicketCreateComponent {
  request: CreateTicketRequest = {
    title: '',
    description: '',
    customerEmail: '',
    productName: ''
  };

  createdTicket: TicketResponse | null = null;
  isSaving = false;
  errorMessage = '';

  constructor(private readonly ticketService: TicketService) {}

  submit(): void {
    this.errorMessage = '';
    this.createdTicket = null;

    if (!this.request.title || !this.request.description || !this.request.customerEmail) {
      this.errorMessage = 'Title, description and customer email are required.';
      return;
    }

    this.isSaving = true;

    this.ticketService.createTicket(this.request).subscribe({
      next: (ticket) => {
        this.createdTicket = ticket;
        this.isSaving = false;
        this.request = {
          title: '',
          description: '',
          customerEmail: '',
          productName: ''
        };
      },
      error: () => {
        this.errorMessage = 'Unable to create ticket. Please check API URL and CORS settings.';
        this.isSaving = false;
      }
    });
  }
}
