export interface ApiRequestLog {
  timestamp: string; // ISO string
  endpoint: string;
  statusCode: number;
}
