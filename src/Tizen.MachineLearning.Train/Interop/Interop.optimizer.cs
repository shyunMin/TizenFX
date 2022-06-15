/*
 * Copyright (c) 2022 Samsung Electronics Co., Ltd All Rights Reserved
 *
 * Licensed under the Apache License, Version 2.0 (the License);
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an AS IS BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Runtime.InteropServices;
using Tizen.MachineLearning.Train;

internal static partial class Interop
{
    internal static partial class Optimizer
    {
        /* int ml_train_optimizer_create(ml_train_optimizer_h *optimizer, ml_train_optimizer_type_e type) */
        [DllImport(Libraries.Nntrainer, EntryPoint = "ml_train_optimizer_create")]
        public static extern NNTrainerError Create(out IntPtr optimizerHandle, NNTrainerOptimizerType type);

        /* int ml_train_optimizer_destroy(ml_train_optimizer_h optimizer) */
        [DllImport(Libraries.Nntrainer, EntryPoint = "ml_train_optimizer_destroy")]
        public static extern NNTrainerError Destroy(IntPtr optimizerHandle);
    }
}