import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {
  username: string | null = null;

  constructor(private route: ActivatedRoute) {}

  ngOnInit() : void {
    this.username = this.route.snapshot.paramMap.get('username');
  }

}
