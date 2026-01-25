import 'package:image_picker/image_picker.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

part 'image_picker_service.g.dart';

class ImagePickerService {
  final ImagePicker _picker;

  ImagePickerService(this._picker);

  Future<String?> pickImageFromCamera() async {
    final XFile? image = await _picker.pickImage(
      source: ImageSource.camera,
      maxWidth: 1920,
      maxHeight: 1080,
      imageQuality: 85,
    );
    return image?.path;
  }

  Future<String?> pickImageFromGallery() async {
    final XFile? image = await _picker.pickImage(
      source: ImageSource.gallery,
      maxWidth: 1920,
      maxHeight: 1080,
      imageQuality: 85,
    );
    return image?.path;
  }
}

@riverpod
ImagePickerService imagePickerService(ImagePickerServiceRef ref) {
  return ImagePickerService(ImagePicker());
}
