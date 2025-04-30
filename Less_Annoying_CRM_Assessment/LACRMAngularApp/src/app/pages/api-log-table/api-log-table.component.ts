import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { ApiRequestLog } from '../../models/api-request-log/api-request-log.module';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ApiLogServiceService } from '../../services/api-log-service.service';

@Component({
  selector: 'app-api-log-table',
  templateUrl: './api-log-table.component.html',
  styleUrl: './api-log-table.component.css'
})
export class ApiLogTableComponent implements OnInit {
  displayedColumns: string[] = ['timestamp', 'endpoint', 'statusCode'];
  dataSource = new MatTableDataSource<ApiRequestLog>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private apiLogService: ApiLogServiceService) {}

  ngOnInit(): void {
    this.apiLogService.getLogs().subscribe(logs => {
      this.dataSource.data = logs;
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }
}
