import { TestBed } from '@angular/core/testing';
import { DocumentValidationService } from './document-validation.service';

describe('DocumentValidationService', () => {
  let service: DocumentValidationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DocumentValidationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('determineCategoryByDestination', () => {
    it('should return Domestic for domestic city', () => {
      expect(service.determineCategoryByDestination('Delhi')).toBe('Domestic');
      expect(service.determineCategoryByDestination('Mumbai')).toBe('Domestic');
      expect(service.determineCategoryByDestination('Bengaluru')).toBe('Domestic');
    });

    it('should return International for international city', () => {
      expect(service.determineCategoryByDestination('Paris')).toBe('International');
      expect(service.determineCategoryByDestination('London')).toBe('International');
      expect(service.determineCategoryByDestination('Tokyo')).toBe('International');
    });

    it('should be case-insensitive', () => {
      expect(service.determineCategoryByDestination('DELHI')).toBe('Domestic');
      expect(service.determineCategoryByDestination('delhi')).toBe('Domestic');
      expect(service.determineCategoryByDestination('PARIS')).toBe('International');
    });

    it('should handle trimmed input', () => {
      expect(service.determineCategoryByDestination('  Delhi  ')).toBe('Domestic');
      expect(service.determineCategoryByDestination('  Paris  ')).toBe('International');
    });

    it('should throw error for unknown cities', () => {
      expect(() => service.determineCategoryByDestination('UnknownCity')).toThrowError(/not a recognized destination/);
      expect(() => service.determineCategoryByDestination('RandomPlace')).toThrowError(/not a recognized destination/);
    });
  });

  describe('getRequiredDocumentType', () => {
    it('should return NationalId for domestic destinations', () => {
      expect(service.getRequiredDocumentType('Delhi')).toBe('NationalId');
      expect(service.getRequiredDocumentType('Mumbai')).toBe('NationalId');
    });

    it('should return Passport for international destinations', () => {
      expect(service.getRequiredDocumentType('Paris')).toBe('Passport');
      expect(service.getRequiredDocumentType('London')).toBe('Passport');
    });
  });

  describe('getDocumentTypeDisplay', () => {
    it('should return display name for NationalId', () => {
      expect(service.getDocumentTypeDisplay('NationalId')).toBe('National ID');
    });

    it('should return display name for Passport', () => {
      expect(service.getDocumentTypeDisplay('Passport')).toBe('Passport');
    });
  });

  describe('validateDocument', () => {
    describe('domestic destination validation', () => {
      it('should accept NationalId for domestic destination', () => {
        const result = service.validateDocument('Delhi', 'NationalId', 'NID-123');
        expect(result.isValid).toBe(true);
        expect(result.message).toBe('Document is valid.');
        expect(result.requiredDocumentType).toBe('NationalId');
      });

      it('should reject Passport for domestic destination', () => {
        const result = service.validateDocument('Delhi', 'Passport', 'PASSPORT-123');
        expect(result.isValid).toBe(false);
        expect(result.message).toBe('Domestic destinations require a National ID.');
        expect(result.requiredDocumentType).toBe('NationalId');
      });
    });

    describe('international destination validation', () => {
      it('should accept Passport for international destination', () => {
        const result = service.validateDocument('Paris', 'Passport', 'PASSPORT-456');
        expect(result.isValid).toBe(true);
        expect(result.message).toBe('Document is valid.');
        expect(result.requiredDocumentType).toBe('Passport');
      });

      it('should reject NationalId for international destination', () => {
        const result = service.validateDocument('Paris', 'NationalId', 'NID-456');
        expect(result.isValid).toBe(false);
        expect(result.message).toBe('International destinations require a Passport.');
        expect(result.requiredDocumentType).toBe('Passport');
      });
    });

    describe('document number validation', () => {
      it('should reject empty document number', () => {
        const result = service.validateDocument('Delhi', 'NationalId', '');
        expect(result.isValid).toBe(false);
        expect(result.message).toBe('Document number is required.');
      });

      it('should reject whitespace-only document number', () => {
        const result = service.validateDocument('Delhi', 'NationalId', '   ');
        expect(result.isValid).toBe(false);
        expect(result.message).toBe('Document number is required.');
      });
    });
  });
});
