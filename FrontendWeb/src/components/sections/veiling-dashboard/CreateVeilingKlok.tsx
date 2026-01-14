import React from 'react';
import {useForm} from 'react-hook-form';
import {useRootContext} from '../../contexts/RootContext';
import {useComponentStateReducer} from '../../../hooks/useComponentStateReducer';
import {createVeilingKlok} from '../../../controllers/server/veilingmeester';
import {CreateVeilingKlokDTO} from '../../../declarations/dtos/input/CreateVeilingKlokDTO';
import Button from '../../buttons/Button';
import FormInputField from '../../form-elements/FormInputField';
import ComponentState from '../../elements/ComponentState';
import {LayoutGroup, motion} from 'framer-motion';
import {isHttpError} from '../../../declarations/types/HttpError';
import {delay, toIsoStringWithOffset} from '../../../utils/standards';
import {VeilingKlokDetailsOutputDto} from "../../../declarations/dtos/output/VeilingKlokDetailsOutputDto";

type Props = {
	onClose: () => void;
	onSuccess?: (klok: VeilingKlokDetailsOutputDto) => void;
};

type CreateVeilingKlokFormState = {
	scheduledAt: string;
	veilingDurationSeconden: number;
};

function CreateVeilingKlok({onClose, onSuccess}: Props) {
	const {t} = useRootContext();
	const [state, updateState] = useComponentStateReducer();

	const {
		register,
		handleSubmit,
		formState: {errors}
	} = useForm<CreateVeilingKlokFormState>();

	const onSubmit = async (data: CreateVeilingKlokFormState) => {
		try {
			updateState({type: 'loading'});

			const payload: CreateVeilingKlokDTO = {
				scheduledAt: toIsoStringWithOffset(new Date(data.scheduledAt)),
				veilingDurationSeconds: data.veilingDurationSeconden,
				products: {}
			};

			const response = await createVeilingKlok(payload);
			updateState({type: 'succeed', message: t('veilingklok_created')});
			await delay(2000);
			if (onSuccess) onSuccess(response.data);
			onClose();

		} catch (err) {
			if (isHttpError(err)) {
				updateState({type: 'error', message: err.message});
			} else {
				updateState({type: 'error', message: t('something_went_wrong')});
			}
			await delay(3000);
			updateState({type: 'idle'});
		}
	};

	return (
		<LayoutGroup>
			<motion.div layout className={'modal-card klok-schedule-card auto-width'} onClick={(e) => e.stopPropagation()}>
				{state.type === 'idle' && (
					<>
						<div className={'klok-schedule-card-header'}>
							<Button className="modal-card-back-btn" icon="bi-x" onClick={() => onClose?.()} type="button" aria-label={t('aria_back_button')}/>
							<div className={'create-product-card-header-text-ctn'}>
								<h1 className={'create-product-card-h1'}>
									<i className="bi bi-calendar-plus-fill create-product-card-header-icon"></i>
									{t('schedule_veilingklok')}
								</h1>
							</div>
						</div>

						<div className="klok-schedule-card-body">
							<form onSubmit={handleSubmit(onSubmit)} className="klok-schedule-card-form">
								<FormInputField
									id="scheduledAt"
									label={t('scheduledDate')}
									type="datetime-local"
									error={errors.scheduledAt?.message}
									{...register('scheduledAt', {required: t('required_field')})}
								/>

								<FormInputField
									id="veilingDurationMinutes"
									label={t('duration_per_bids')}
									type="number"
									error={errors.veilingDurationSeconden?.message}
									{...register('veilingDurationSeconden', {required: t('required_field'), valueAsNumber: true})}
								/>

								<Button
									type="submit"
									icon="bi-plus-lg"
									label={t('create')}
								/>
							</form>
						</div>
					</>
				)}

				{state.type !== 'idle' && <ComponentState state={state}/>}

			</motion.div>
		</LayoutGroup>
	);
}

export default CreateVeilingKlok;
