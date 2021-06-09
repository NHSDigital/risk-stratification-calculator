import { qCovidClient as client } from '../../../helpers/client';
import { CreateBundle } from '../../../helpers/createBundle';
import {
  age,
  sex,
  ethnicGroup,
  height,
  weight,
  cerebralPalsy,
  housing,
  postcode,
  atrialFibrillation,
  dementia,
  cirrhosisOfLiver,
  parkinsons
} from '../../../helpers/snomedCodes';
import {
  diabetes,
  arthritis,
  bloodCancer,
  boneMarrowTransplant,
  congenitalHeartDisease,
  coronaryHeartDisease,
  cysticFibrosisBronchiectasisAlveolitis,
  epilepsy,
  heartFailure,
  asthma,
  fracture,
  learningDisabilityOrDownsSyndrome,
  lungOrOralCancer,
  motorNeuronDisease,
  organTransplant,
  peripheralVascularDisease,
  pulmonaryDisease,
  pulmonaryHypertensionOrPulmonaryFibrosis,
  radiotherapy,
  renal,
  severalMentalIllness,
  sickleCellDisease,
  stroke,
  thrombosis,
  antiLeukotriene,
  immunosuppressants,
  chemotherapy,
  oralPrednisolone
} from '../../../helpers/qCovidCodes';

import { Prediction } from '../../../helpers/riskAssessment_Prediction';

describe('A risk assessment with a score is obtained when a patient\'s data is submited ', () => {
  it('Score for FEMALE_WITH_100_CHANCEDYING', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 53, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.female.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianOther.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.careHome.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, 'W1G8YS');

    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 86, weight.unit);

    bundleCreator.addObservationValueBoolean(atrialFibrillation.id, atrialFibrillation.snomedCode);
    bundleCreator.addObservationValueBoolean(coronaryHeartDisease.id,
      coronaryHeartDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(heartFailure.id, heartFailure.qcovidCode);
    bundleCreator.addObservationValueBoolean(congenitalHeartDisease.id,
      congenitalHeartDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(pulmonaryHypertensionOrPulmonaryFibrosis.id,
      pulmonaryHypertensionOrPulmonaryFibrosis.qcovidCode);
    bundleCreator.addObservationValueBoolean(cysticFibrosisBronchiectasisAlveolitis.id,
      cysticFibrosisBronchiectasisAlveolitis.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(diabetes.id, diabetes.qcovidCode,
      diabetes.type1.qcovidCode);
    bundleCreator.addObservationValueBoolean(fracture.id, fracture.qcovidCode);

    bundleCreator.addObservationValueCodeableConcept(renal.id, renal.qcovidCode,
      renal.ckdTransplant.qcovidCode);
    bundleCreator.addObservationValueBoolean(dementia.id, dementia.snomedCode);
    bundleCreator.addObservationValueBoolean(cerebralPalsy.id,
      cerebralPalsy.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(learningDisabilityOrDownsSyndrome.id,
      learningDisabilityOrDownsSyndrome.snomedCode,
      learningDisabilityOrDownsSyndrome.learningDisabilityExcludingDownsSyndrome.qcovidCode);
    bundleCreator.addObservationValueBoolean(sickleCellDisease.id, sickleCellDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(thrombosis.id, thrombosis.qcovidCode);
    bundleCreator.addObservationValueBoolean(organTransplant.id, organTransplant.qcovidCode);
    bundleCreator.addObservationValueBoolean(bloodCancer.id, bloodCancer.qcovidCode);
    bundleCreator.addObservationValueBoolean(boneMarrowTransplant.id,
      boneMarrowTransplant.qcovidCode);
    bundleCreator.addObservationValueBoolean(radiotherapy.id, radiotherapy.qcovidCode);
    bundleCreator.addObservationValueBoolean(immunosuppressants.id,
      immunosuppressants.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id,
      chemotherapy.qcovidCode,
      chemotherapy.none.qcovidCode);

    const response = await client.postSuccess(bundleCreator.bundle);

    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(100, 28097.780275358247);
    expectedPred.addHospitalisation(100, 2400.03840061441);
    expectedPred.addBaselineDeath(0.003559);
    expectedPred.addBaselineHospitalisation(0.041666);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for MALE_WITH_BARELY_ANY_CHANCE_OF_DYING', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 25, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.male.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.blackAfricanAndWhite.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.independenent.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, 'GU211AH');

    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 163, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 73, weight.unit);

    bundleCreator.addObservationValueBoolean(atrialFibrillation.id, atrialFibrillation.snomedCode);
    bundleCreator.addObservationValueBoolean(coronaryHeartDisease.id,
      coronaryHeartDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(congenitalHeartDisease.id,
      congenitalHeartDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(peripheralVascularDisease.id,
      peripheralVascularDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(asthma.id, asthma.qcovidCode);
    bundleCreator.addObservationValueBoolean(antiLeukotriene.id,
      antiLeukotriene.qcovidCode);
    bundleCreator.addObservationValueBoolean(pulmonaryHypertensionOrPulmonaryFibrosis.id,
      pulmonaryHypertensionOrPulmonaryFibrosis.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(diabetes.id, diabetes.qcovidCode,
      diabetes.none.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(renal.id, renal.qcovidCode,
      renal.none.qcovidCode);
    bundleCreator.addObservationValueBoolean(cirrhosisOfLiver.id, cirrhosisOfLiver.snomedCode);
    bundleCreator.addObservationValueBoolean(epilepsy.id, epilepsy.qcovidCode);
    bundleCreator.addObservationValueBoolean(motorNeuronDisease.id, motorNeuronDisease.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(learningDisabilityOrDownsSyndrome.id,
      learningDisabilityOrDownsSyndrome.snomedCode,
      learningDisabilityOrDownsSyndrome.none.qcovidCode);
    bundleCreator.addObservationValueBoolean(organTransplant.id, organTransplant.qcovidCode);
    bundleCreator.addObservationValueBoolean(bloodCancer.id, bloodCancer.qcovidCode);
    bundleCreator.addObservationValueBoolean(oralPrednisolone.id,
      oralPrednisolone.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id,
      chemotherapy.qcovidCode,
      chemotherapy.abiraterone.qcovidCode);

    const response = await client.postSuccess(bundleCreator.bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundleCreator.bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(0.02696, 137.55102040816325);
    expectedPred.addHospitalisation(4.143676, 509.3639827904118);
    expectedPred.addBaselineDeath(0.000196);
    expectedPred.addBaselineHospitalisation(0.008135);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for OVER80s_FEMALE', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 86, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.female.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.blackCaribbean.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.independenent.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, 'SY83HE');

    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 159, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 63, weight.unit);

    bundleCreator.addObservationValueBoolean(coronaryHeartDisease.id,
      coronaryHeartDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(heartFailure.id, heartFailure.qcovidCode);
    bundleCreator.addObservationValueBoolean(congenitalHeartDisease.id,
      congenitalHeartDisease.qcovidCode);

    bundleCreator.addObservationValueBoolean(asthma.id, asthma.qcovidCode);
    bundleCreator.addObservationValueBoolean(antiLeukotriene.id,
      antiLeukotriene.qcovidCode);
    bundleCreator.addObservationValueBoolean(cysticFibrosisBronchiectasisAlveolitis.id,
      cysticFibrosisBronchiectasisAlveolitis.qcovidCode);
    bundleCreator.addObservationValueBoolean(pulmonaryHypertensionOrPulmonaryFibrosis.id,
      pulmonaryHypertensionOrPulmonaryFibrosis.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(diabetes.id, diabetes.qcovidCode,
      diabetes.type2.qcovidCode);
    bundleCreator.addObservationValueBoolean(fracture.id, fracture.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(renal.id, renal.qcovidCode,
      renal.none.qcovidCode);
    bundleCreator.addObservationValueBoolean(cirrhosisOfLiver.id, cirrhosisOfLiver.snomedCode);
    bundleCreator.addObservationValueBoolean(epilepsy.id, epilepsy.qcovidCode);
    bundleCreator.addObservationValueBoolean(parkinsons.id, parkinsons.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(learningDisabilityOrDownsSyndrome.id,
      learningDisabilityOrDownsSyndrome.snomedCode,
      learningDisabilityOrDownsSyndrome.none.qcovidCode);
    bundleCreator.addObservationValueBoolean(thrombosis.id, thrombosis.qcovidCode);
    bundleCreator.addObservationValueBoolean(lungOrOralCancer.id, lungOrOralCancer.qcovidCode);
    bundleCreator.addObservationValueBoolean(oralPrednisolone.id,
      oralPrednisolone.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id,
      chemotherapy.qcovidCode,
      chemotherapy.anthracyclineBasedRegimens.qcovidCode);

    const response = await client.postSuccess(bundleCreator.bundle);

    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(31.279878, 248.86726762087375);
    expectedPred.addHospitalisation(97.994604, 401.630397717958);
    expectedPred.addBaselineDeath(0.125689);
    expectedPred.addBaselineHospitalisation(0.243992);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for YOUNG_MALE_IN_CARE_HOME_NO_LONGER_EXISTING_POSTCODE', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 30, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.male.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianOther.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.homeless.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, 'BS182LJ');

    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 150, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 90, weight.unit);
    bundleCreator.addObservationValueBoolean(atrialFibrillation.id, atrialFibrillation.snomedCode);
    bundleCreator.addObservationValueBoolean(congenitalHeartDisease.id,
      congenitalHeartDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(pulmonaryDisease.id, pulmonaryDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(pulmonaryHypertensionOrPulmonaryFibrosis.id,
      pulmonaryHypertensionOrPulmonaryFibrosis.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(diabetes.id, diabetes.qcovidCode,
      diabetes.none.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(renal.id, renal.qcovidCode,
      renal.ckd5NoDyalisys.qcovidCode);
    bundleCreator.addObservationValueBoolean(motorNeuronDisease.id, motorNeuronDisease.snomedCode);
    bundleCreator.addObservationValueBoolean(parkinsons.id, parkinsons.snomedCode);
    bundleCreator.addObservationValueBoolean(dementia.id, dementia.snomedCode);
    bundleCreator.addObservationValueBoolean(cerebralPalsy.id, cerebralPalsy.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(learningDisabilityOrDownsSyndrome.id,
      learningDisabilityOrDownsSyndrome.snomedCode,
      learningDisabilityOrDownsSyndrome.downsSyndrome.qcovidCode);
    bundleCreator.addObservationValueBoolean(thrombosis.id, thrombosis.qcovidCode);
    bundleCreator.addObservationValueBoolean(radiotherapy.id, radiotherapy.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id,
      chemotherapy.qcovidCode,
      chemotherapy.bendamustine.qcovidCode);

    const response = await client.postSuccess(bundleCreator.bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundleCreator.bundle.entry.map((e) => e.resource))
    );
    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(32.325485, 82253.14249363868);
    expectedPred.addHospitalisation(99.999997, 6851.191901890929);
    expectedPred.addBaselineDeath(0.000393);
    expectedPred.addBaselineHospitalisation(0.014596);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for TEENAGER_FEMALE_WITH_100_CHANCEDYING', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 19, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.female.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.blackOther.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.homeless.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, 'BS42QE');

    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 170, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 70, weight.unit);
    bundleCreator.addObservationValueBoolean(coronaryHeartDisease.id,
      coronaryHeartDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(congenitalHeartDisease.id,
      congenitalHeartDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(antiLeukotriene.id,
      antiLeukotriene.qcovidCode);
    bundleCreator.addObservationValueBoolean(pulmonaryDisease.id, pulmonaryDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(cysticFibrosisBronchiectasisAlveolitis.id,
      cysticFibrosisBronchiectasisAlveolitis.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(diabetes.id, diabetes.qcovidCode,
      diabetes.type2.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(renal.id, renal.qcovidCode,
      renal.ckd5Dyalisys.qcovidCode);
    bundleCreator.addObservationValueBoolean(stroke.id, stroke.qcovidCode);
    bundleCreator.addObservationValueBoolean(parkinsons.id, parkinsons.snomedCode);
    bundleCreator.addObservationValueBoolean(cerebralPalsy.id, cerebralPalsy.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(learningDisabilityOrDownsSyndrome.id,
      learningDisabilityOrDownsSyndrome.snomedCode,
      learningDisabilityOrDownsSyndrome.downsSyndrome.qcovidCode);
    bundleCreator.addObservationValueBoolean(arthritis.id, arthritis.qcovidCode);
    bundleCreator.addObservationValueBoolean(sickleCellDisease.id, sickleCellDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(lungOrOralCancer.id, lungOrOralCancer.qcovidCode);
    bundleCreator.addObservationValueBoolean(organTransplant.id, organTransplant.qcovidCode);
    bundleCreator.addObservationValueBoolean(boneMarrowTransplant.id,
      boneMarrowTransplant.qcovidCode);
    bundleCreator.addObservationValueBoolean(radiotherapy.id, radiotherapy.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id,
      chemotherapy.qcovidCode,
      chemotherapy.cisplatinBasedRegimens.qcovidCode);
    const response = await client.postSuccess(bundleCreator.bundle);

    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(100, 418410.04184100416);
    expectedPred.addHospitalisation(100, 5651.633322030067);
    expectedPred.addBaselineDeath(0.000239);
    expectedPred.addBaselineHospitalisation(0.017694);

    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for UNDERWEIGHT_OVER80S_FEMALE', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 81, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.female.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.blackAfricanAndWhite.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.independenent.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, 'LN65UW');

    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 155, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 40, weight.unit);

    bundleCreator.addObservationValueBoolean(peripheralVascularDisease.id,
      peripheralVascularDisease.qcovidCode);

    bundleCreator.addObservationValueBoolean(pulmonaryHypertensionOrPulmonaryFibrosis.id,
      pulmonaryHypertensionOrPulmonaryFibrosis.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(diabetes.id, diabetes.qcovidCode,
      diabetes.type2.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(renal.id, renal.qcovidCode,
      renal.ckd5NoDyalisys.qcovidCode);
    bundleCreator.addObservationValueBoolean(cirrhosisOfLiver.id, cirrhosisOfLiver.snomedCode);

    bundleCreator.addObservationValueBoolean(parkinsons.id, parkinsons.snomedCode);
    bundleCreator.addObservationValueBoolean(cerebralPalsy.id, cerebralPalsy.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(learningDisabilityOrDownsSyndrome.id,
      learningDisabilityOrDownsSyndrome.snomedCode,
      learningDisabilityOrDownsSyndrome.learningDisabilityExcludingDownsSyndrome.qcovidCode);
    bundleCreator.addObservationValueBoolean(severalMentalIllness.id,
      severalMentalIllness.qcovidCode);
    bundleCreator.addObservationValueBoolean(thrombosis.id, thrombosis.qcovidCode);
    bundleCreator.addObservationValueBoolean(bloodCancer.id, bloodCancer.qcovidCode);
    bundleCreator.addObservationValueBoolean(oralPrednisolone.id,
      oralPrednisolone.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id,
      chemotherapy.qcovidCode,
      chemotherapy.none.qcovidCode);

    const response = await client.postSuccess(bundleCreator.bundle);

    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(48.791971, 600.7383772469835);
    expectedPred.addHospitalisation(83.238886, 472.53206551048794);
    expectedPred.addBaselineDeath(0.08122);
    expectedPred.addBaselineHospitalisation(0.176155);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for SCOTTISH_FEMALE', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 32, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.female.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.whiteIrish.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.homeless.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, 'AB42 5TH');

    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 190, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 130, weight.unit);

    bundleCreator.addObservationValueBoolean(heartFailure.id, heartFailure.qcovidCode);
    bundleCreator.addObservationValueBoolean(peripheralVascularDisease.id,
      peripheralVascularDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(asthma.id, asthma.qcovidCode);
    bundleCreator.addObservationValueBoolean(antiLeukotriene.id,
      antiLeukotriene.qcovidCode);

    bundleCreator.addObservationValueBoolean(cysticFibrosisBronchiectasisAlveolitis.id,
      cysticFibrosisBronchiectasisAlveolitis.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(diabetes.id, diabetes.qcovidCode,
      diabetes.none.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(renal.id, renal.qcovidCode,
      renal.ckd5NoDyalisys.qcovidCode);
    bundleCreator.addObservationValueBoolean(cirrhosisOfLiver.id, cirrhosisOfLiver.snomedCode);
    bundleCreator.addObservationValueBoolean(epilepsy.id, epilepsy.qcovidCode);
    bundleCreator.addObservationValueBoolean(stroke.id, stroke.qcovidCode);
    bundleCreator.addObservationValueBoolean(parkinsons.id, parkinsons.snomedCode);
    bundleCreator.addObservationValueBoolean(dementia.id, dementia.snomedCode);
    bundleCreator.addObservationValueBoolean(cerebralPalsy.id, cerebralPalsy.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(learningDisabilityOrDownsSyndrome.id,
      learningDisabilityOrDownsSyndrome.snomedCode,
      learningDisabilityOrDownsSyndrome.learningDisabilityExcludingDownsSyndrome.qcovidCode);
    bundleCreator.addObservationValueBoolean(thrombosis.id, thrombosis.qcovidCode);
    bundleCreator.addObservationValueBoolean(organTransplant.id, organTransplant.qcovidCode);
    bundleCreator.addObservationValueBoolean(oralPrednisolone.id,
      oralPrednisolone.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id,
      chemotherapy.qcovidCode,
      chemotherapy.dhap.qcovidCode);

    const response = await client.postSuccess(bundleCreator.bundle);

    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(2.316364, 4766.181069958848);
    expectedPred.addHospitalisation(96.042234, 4407.831199228969);
    expectedPred.addBaselineDeath(0.000486);
    expectedPred.addBaselineHospitalisation(0.021789);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for WELSH_MALE', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 31, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.male.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.notStated.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.homeless.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, 'CF409YE');

    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 158, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 63, weight.unit);

    bundleCreator.addObservationValueBoolean(atrialFibrillation.id, atrialFibrillation.snomedCode);
    bundleCreator.addObservationValueBoolean(heartFailure.id, heartFailure.qcovidCode);
    bundleCreator.addObservationValueBoolean(asthma.id, asthma.qcovidCode);
    bundleCreator.addObservationValueBoolean(antiLeukotriene.id,
      antiLeukotriene.qcovidCode);
    bundleCreator.addObservationValueBoolean(pulmonaryDisease.id, pulmonaryDisease.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(diabetes.id, diabetes.qcovidCode,
      diabetes.type1.qcovidCode);
    bundleCreator.addObservationValueBoolean(fracture.id, fracture.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(renal.id, renal.qcovidCode,
      renal.ckd5NoDyalisys.qcovidCode);
    bundleCreator.addObservationValueBoolean(epilepsy.id, epilepsy.qcovidCode);
    bundleCreator.addObservationValueBoolean(motorNeuronDisease.id, motorNeuronDisease.snomedCode);
    bundleCreator.addObservationValueBoolean(cerebralPalsy.id, cerebralPalsy.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(learningDisabilityOrDownsSyndrome.id,
      learningDisabilityOrDownsSyndrome.snomedCode,
      learningDisabilityOrDownsSyndrome.none.qcovidCode);
    bundleCreator.addObservationValueBoolean(organTransplant.id, organTransplant.qcovidCode);
    bundleCreator.addObservationValueBoolean(bloodCancer.id, bloodCancer.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id,
      chemotherapy.qcovidCode,
      chemotherapy.trifuradine.qcovidCode);

    const response = await client.postSuccess(bundleCreator.bundle);

    expect(response.contained.find((el) => el.id === 'postcode')).toEqual(undefined);

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(1.904711, 4223.30598669623);
    expectedPred.addHospitalisation(90.005966, 5622.91285062785);
    expectedPred.addBaselineDeath(0.000451);
    expectedPred.addBaselineHospitalisation(0.016007);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for NORTHERNIRISH_OBESE_FEMALE', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 79, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.female.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.blackAfricanAndWhite.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.homeless.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, 'BT424BU');

    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 210, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 150, weight.unit);

    bundleCreator.addObservationValueBoolean(congenitalHeartDisease.id,
      congenitalHeartDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(asthma.id, asthma.qcovidCode);
    bundleCreator.addObservationValueBoolean(antiLeukotriene.id,
      antiLeukotriene.qcovidCode);
    bundleCreator.addObservationValueBoolean(pulmonaryDisease.id, pulmonaryDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(cysticFibrosisBronchiectasisAlveolitis.id,
      cysticFibrosisBronchiectasisAlveolitis.qcovidCode);

    bundleCreator.addObservationValueCodeableConcept(diabetes.id, diabetes.qcovidCode,
      diabetes.type2.qcovidCode);
    bundleCreator.addObservationValueBoolean(fracture.id, fracture.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(renal.id, renal.qcovidCode,
      renal.ckd4.qcovidCode);
    bundleCreator.addObservationValueBoolean(epilepsy.id, epilepsy.qcovidCode);
    bundleCreator.addObservationValueBoolean(stroke.id, stroke.qcovidCode);

    bundleCreator.addObservationValueCodeableConcept(learningDisabilityOrDownsSyndrome.id,
      learningDisabilityOrDownsSyndrome.snomedCode,
      learningDisabilityOrDownsSyndrome.learningDisabilityExcludingDownsSyndrome.qcovidCode);
    bundleCreator.addObservationValueBoolean(organTransplant.id, organTransplant.qcovidCode);
    bundleCreator.addObservationValueBoolean(bloodCancer.id, bloodCancer.qcovidCode);
    bundleCreator.addObservationValueBoolean(immunosuppressants.id,
      immunosuppressants.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id,
      chemotherapy.qcovidCode,
      chemotherapy.none.qcovidCode);

    const response = await client.postSuccess(bundleCreator.bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundleCreator.bundle.entry.map((e) => e.resource))
    );

    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(8.291775, 123.90022862095243);
    expectedPred.addHospitalisation(54.595469, 351.0375692810205);
    expectedPred.addBaselineDeath(0.066923);
    expectedPred.addBaselineHospitalisation(0.155526);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });

  it('Score for FEMALE_LIVING_IN_DEPRIVED_AREA', async () => {
    const bundleCreator = new CreateBundle();
    bundleCreator.addObservationValueQuantity(age.id, age.snomedCode, 78, age.unit);
    bundleCreator.addObservationValueCodeableConcept(sex.id, sex.snomedCode,
      sex.female.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(ethnicGroup.id, ethnicGroup.snomedCode,
      ethnicGroup.asianIndian.snomedCode);
    bundleCreator.addObservationValueCodeableConcept(housing.id, housing.snomedCode,
      housing.independenent.snomedCode);
    bundleCreator.addObservationValueString(postcode.id, postcode.snomedCode, 'OL104HA');

    bundleCreator.addObservationValueQuantity(height.id, height.snomedCode, 140, height.unit);
    bundleCreator.addObservationValueQuantity(weight.id, weight.snomedCode, 47, weight.unit);

    bundleCreator.addObservationValueBoolean(antiLeukotriene.id,
      antiLeukotriene.qcovidCode);
    bundleCreator.addObservationValueBoolean(pulmonaryDisease.id, pulmonaryDisease.qcovidCode);
    bundleCreator.addObservationValueBoolean(cysticFibrosisBronchiectasisAlveolitis.id,
      cysticFibrosisBronchiectasisAlveolitis.qcovidCode);
    bundleCreator.addObservationValueBoolean(pulmonaryHypertensionOrPulmonaryFibrosis.id,
      pulmonaryHypertensionOrPulmonaryFibrosis.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(diabetes.id, diabetes.qcovidCode,
      diabetes.none.qcovidCode);
    bundleCreator.addObservationValueBoolean(fracture.id, fracture.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(renal.id, renal.qcovidCode,
      renal.ckd3.qcovidCode);
    bundleCreator.addObservationValueBoolean(stroke.id, stroke.qcovidCode);
    bundleCreator.addObservationValueBoolean(motorNeuronDisease.id, motorNeuronDisease.snomedCode);
    bundleCreator.addObservationValueBoolean(dementia.id, dementia.snomedCode);

    bundleCreator.addObservationValueCodeableConcept(learningDisabilityOrDownsSyndrome.id,
      learningDisabilityOrDownsSyndrome.snomedCode,
      learningDisabilityOrDownsSyndrome.learningDisabilityExcludingDownsSyndrome.qcovidCode);
    bundleCreator.addObservationValueBoolean(severalMentalIllness.id,
      severalMentalIllness.qcovidCode);
    bundleCreator.addObservationValueBoolean(boneMarrowTransplant.id,
      boneMarrowTransplant.qcovidCode);
    bundleCreator.addObservationValueBoolean(radiotherapy.id, radiotherapy.qcovidCode);
    bundleCreator.addObservationValueCodeableConcept(chemotherapy.id,
      chemotherapy.qcovidCode,
      chemotherapy.none.qcovidCode);

    const response = await client.postSuccess(bundleCreator.bundle);

    expect(response.contained).toEqual(
      expect.arrayContaining(bundleCreator.bundle.entry.map((e) => e.resource))
    );
    const expectedPred = new Prediction();
    expectedPred.addDeathAttributedToCovid(41.013823, 677.4777085845489);
    expectedPred.addHospitalisation(37.245529, 254.5536677214541);
    expectedPred.addBaselineDeath(0.060539);
    expectedPred.addBaselineHospitalisation(0.146317);
    expect(response.prediction).toEqual(expectedPred.prediction);
  });
});
