import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MembersService } from '../_services/members.service';
import { Member } from '../_models/member';
import { DatePipe, NgIf } from '@angular/common';
import { AgePipe } from '../_pipes/age.pipe';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [RouterModule, NgIf, DatePipe, AgePipe ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit {

  private memberService = inject(MembersService);
  private route = inject(ActivatedRoute);
  member?: Member;
  username: string | null = null;

  constructor(route: ActivatedRoute) {}

  ngOnInit() : void {
    this.loadMember();
  }
   
  loadMember() {
    const username = this.route.snapshot.paramMap.get('username');

    if (!username) return;

    this.memberService.getMember(username).subscribe({
      next: member => this.member = member
    })

  }

}
