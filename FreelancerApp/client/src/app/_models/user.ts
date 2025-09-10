export interface User {
    username: string;
    token: string;
    roles: string[];
    photoUrl?: string;
    knownAs: string;
    IsAccountDisabled: boolean;
}