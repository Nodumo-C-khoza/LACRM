import 'datatables.net';

declare global {
  interface JQuery {
    DataTable: any;
  }
}
