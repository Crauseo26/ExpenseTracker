class UserCredentials {
  final String email;
  final String password;

  UserCredentials({
    required this.email,
    required this.password,
  });

  Map<String, dynamic> toJson() => {
    'email': email,
    'password': password,
  };
}
