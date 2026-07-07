export interface CreateTicketRequest {
  title: string;
  description: string;
  customerEmail: string;
  productName?: string | null;
}

export interface ClassificationResult {
  category: string;
  priority: string;
  routedTeam: string;
  confidence: number;
  reason: string;
  tags: string[];
}

export interface TicketResponse {
  id: number;
  title: string;
  description: string;
  customerEmail: string;
  productName?: string | null;
  status: string;
  createdAtUtc: string;
  classification?: ClassificationResult | null;
}
