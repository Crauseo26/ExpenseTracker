import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'core/utils/image_picker_service.dart';

void main() {
  runApp(
    const ProviderScope(
      child: MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'FinancIA',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      home: const HomeScreen(),
    );
  }
}

class HomeScreen extends ConsumerStatefulWidget {
  const HomeScreen({super.key});

  @override
  ConsumerState<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends ConsumerState<HomeScreen> {
  String? _imagePath;

  @override
  Widget build(BuildContext context) {
    final imagePicker = ref.watch(imagePickerServiceProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('FinancIA - Mobile')),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            if (_imagePath != null) ...[
              const Text('Selected Image:'),
              Padding(
                padding: const EdgeInsets.all(8.0),
                child: Text(_imagePath!, style: const TextStyle(fontSize: 12)),
              ),
              const SizedBox(height: 20),
            ],
            ElevatedButton.icon(
              onPressed: () async {
                final path = await imagePicker.pickImageFromCamera();
                if (path != null) setState(() => _imagePath = path);
              },
              icon: const Icon(Icons.camera_alt),
              label: const Text('Take Photo'),
            ),
            const SizedBox(height: 10),
            ElevatedButton.icon(
              onPressed: () async {
                final path = await imagePicker.pickImageFromGallery();
                if (path != null) setState(() => _imagePath = path);
              },
              icon: const Icon(Icons.photo_library),
              label: const Text('Pick from Gallery'),
            ),
          ],
        ),
      ),
    );
  }
}