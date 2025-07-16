import { NgFor } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgFor],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  http = inject(HttpClient);
  title = 'FreelancerApp';
  users: any;

  ngOnInit(): void {
    setTimeout(() => {
      console.log('ngOnInit (delayed) running');

      // Add a breakpoint here
      this.http.get('https://localhost:5001/api/users')
        .subscribe({
          next: response => this.users = response, // sets the api response to variable users of type any
          error: error => console.log(error),
          complete: () => console.log('Request has completed')
        })

    }, 1000); // a second delay

    console.log('ngOnInit running')
  }
}
