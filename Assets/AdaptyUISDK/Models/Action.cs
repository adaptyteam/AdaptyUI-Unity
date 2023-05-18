using System;

namespace AdaptySDK {
    public static partial class AdaptyUI {
        public enum ActionType {
            /// Close Button was pressed by user.
            Close,

            /// Some button which contains url (e.g. Terms or Privacy & Policy) was pressed by user.
            OpenUrl,

            /// Some button with custom action (e.g. Login) was pressed by user.
            Custom,

            /// Android Back Button was pressed by user.
            AndroidSystemBack,
        }

        public partial class Action {
            /// The type of action.
            public readonly ActionType Type;

            /// Additional value of action. Look here in case of `OpenUrl` or `Custom` types.
            public readonly string Value;
        }
    }
}