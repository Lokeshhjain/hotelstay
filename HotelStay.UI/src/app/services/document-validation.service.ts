import { Injectable } from '@angular/core';

export type DocumentType = 'NationalId' | 'Passport';

export interface DocumentValidationResult {
  isValid: boolean;
  message: string;
  requiredDocumentType: DocumentType | null;
}

@Injectable({
  providedIn: 'root'
})
export class DocumentValidationService {
  readonly domesticCities = [
    'india', 'delhi', 'mumbai', 'bengaluru', 'bangalore', 'chennai', 'hyderabad'
  ];
  
  readonly internationalCities = [
    'london', 'paris', 'tokyo', 'new york', 'toronto', 'sydney'
  ];

  /**
   * Determines if a destination is domestic or international
   * by matching against known city lists.
   * Throws error if destination is not a known city.
   */
  determineCategoryByDestination(destination: string): 'Domestic' | 'International' {
    const normalized = destination.trim().toLowerCase();
    
    // Check for exact match in domestic cities
    const isDomestic = this.domesticCities.some(city => 
      normalized === city
    );
    
    if (isDomestic) {
      return 'Domestic';
    }

    // Check for exact match in international cities
    const isInternational = this.internationalCities.some(city =>
      normalized === city
    );

    if (isInternational) {
      return 'International';
    }

    // Destination not found in either list
    throw new Error(`"${destination}" is not a recognized destination. Valid destinations: ${[...this.domesticCities, ...this.internationalCities].join(', ')}`);
  }

  /**
   * Returns the required document type for a destination.
   * - Domestic: NationalId
   * - International: Passport
   * Throws error if destination is not recognized.
   */
  getRequiredDocumentType(destination: string): DocumentType {
    const category = this.determineCategoryByDestination(destination);
    return category === 'Domestic' ? 'NationalId' : 'Passport';
  }

  /**
   * Safely determines the category, returning null if destination is invalid.
   * Does not throw an error - returns null for unknown destinations.
   */
  tryDetermineCategoryByDestination(destination: string): 'Domestic' | 'International' | null {
    try {
      return this.determineCategoryByDestination(destination);
    } catch {
      return null;
    }
  }

  /**
   * Gets a display-friendly string for a document type.
   */
  getDocumentTypeDisplay(docType: DocumentType): string {
    return docType === 'NationalId' ? 'National ID' : 'Passport';
  }

  /**
   * Validates if a submitted document matches the destination requirement.
   * Returns validation result with error message if destination is invalid or document doesn't match.
   */
  validateDocument(
    destination: string,
    submittedDocumentType: DocumentType,
    documentNumber: string
  ): DocumentValidationResult {
    try {
      // Validate destination is a known city
      const category = this.determineCategoryByDestination(destination);
      
      // Validate document number is not empty
      if (!documentNumber || documentNumber.trim() === '') {
        return {
          isValid: false,
          message: 'Document number is required.',
          requiredDocumentType: category === 'Domestic' ? 'NationalId' : 'Passport'
        };
      }

      // Get required document type
      const requiredType = category === 'Domestic' ? 'NationalId' : 'Passport';

      // Check if submitted document matches requirement
      if (submittedDocumentType !== requiredType) {
        return {
          isValid: false,
          message: `${category} destinations require a ${this.getDocumentTypeDisplay(requiredType)}.`,
          requiredDocumentType: requiredType
        };
      }

      // Document is valid
      return {
        isValid: true,
        message: 'Document is valid.',
        requiredDocumentType: requiredType
      };
    } catch (error: any) {
      // Destination is not recognized
      return {
        isValid: false,
        message: error?.message || 'Invalid destination.',
        requiredDocumentType: null
      };
    }
  }
}
