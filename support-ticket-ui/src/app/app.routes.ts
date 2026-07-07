import { Routes } from '@angular/router';
import { TicketCreateComponent } from './pages/ticket-create/ticket-create.component';
import { TicketListComponent } from './pages/ticket-list/ticket-list.component';

export const routes: Routes = [
  { path: '', redirectTo: 'tickets/new', pathMatch: 'full' },
  { path: 'tickets/new', component: TicketCreateComponent },
  { path: 'tickets', component: TicketListComponent },
  { path: '**', redirectTo: 'tickets/new' }
];
