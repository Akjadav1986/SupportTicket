import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateTicketRequest, TicketResponse } from '../models/ticket.model';

@Injectable({ providedIn: 'root' })
export class TicketService {
  private readonly apiUrl = 'https://localhost:7008/api/tickets';

  constructor(private readonly http: HttpClient) {}

  getTickets(): Observable<TicketResponse[]> {
    return this.http.get<TicketResponse[]>(this.apiUrl);
  }

  createTicket(request: CreateTicketRequest): Observable<TicketResponse> {
    return this.http.post<TicketResponse>(this.apiUrl, request);
  }

  reclassify(id: number): Observable<TicketResponse> {
    return this.http.post<TicketResponse>(`${this.apiUrl}/${id}/classify`, {});
  }
}
