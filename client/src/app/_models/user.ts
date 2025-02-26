export interface User {
   username: string;
   knownAs: string;
   token: string;
   photoUrl?: string;
   bgUrl?: string;
   roles: String[];
}